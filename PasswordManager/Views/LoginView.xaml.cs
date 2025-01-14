using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private const string ProfilesDirectory = "Profiles";
        private bool LoggedIn = false;

        public LoginView()
        {
            InitializeComponent();
            LoadProfiles();
        }

        private void btnNewProfile_Click(object sender, RoutedEventArgs e)
        {
            NewProfileWindow newProfileWindow = new NewProfileWindow();

            Application.Current.MainWindow?.Dispatcher.Invoke(() =>
            {
                newProfileWindow.Owner = Application.Current.MainWindow;
            });

            if (newProfileWindow.ShowDialog() == true)
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    Directory.CreateDirectory(ProfilesDirectory);
                }

                string profilePath = System.IO.Path.Combine(ProfilesDirectory, $"{newProfileWindow.ProfileName}.psmgr");

                var encryptionManager = new EncryptionManager(EncryptionMethod.SHA256);
                string hashedPassword = encryptionManager.Hash(newProfileWindow.ProfilePassword);

                // Zapisujemy metodę szyfrowania jako prefiks
                string defaultEncryptionMethod = EncryptionMethod.AES.ToString();
                File.WriteAllText(profilePath, $"{defaultEncryptionMethod}|{hashedPassword}");

                cbxProfileSelector.Items.Add(newProfileWindow.ProfileName);

                MessageBox.Show($"Profil '{newProfileWindow.ProfileName}' został utworzony.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }




        private void LoadProfiles()
        {
            if (!Directory.Exists(ProfilesDirectory))
            {
                Directory.CreateDirectory(ProfilesDirectory);
            }

          
            cbxProfileSelector.Items.Clear();

            
            var profileFiles = Directory.GetFiles(ProfilesDirectory, "*.psmgr");
            foreach (var file in profileFiles)
            {
                cbxProfileSelector.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
        }



        
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string selectedProfile = cbxProfileSelector.Text;
            string password = PasswordField.Password;

            if (string.IsNullOrEmpty(selectedProfile))
            {
                MessageBox.Show("Wybierz profil.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Wprowadź hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AuthenticateProfile(selectedProfile, password))
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.btnNavLogout.IsEnabled = true;
                    mainWindow.btnNavOptions.IsEnabled = true;
                    mainWindow.btnNavPasswords.IsEnabled = true;

                }
                var mainViewModel = (MainViewModel)((MainWindow)Window.GetWindow(this)).DataContext;
                mainViewModel.CurrentProfile.ProfileName = selectedProfile;
                mainViewModel.NavigateCommand.Execute("Passwords");

            }
            else
            {
                MessageBox.Show("Nieprawidłowe hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateProfile(string profileName, string password)
        {
            string profilePath = System.IO.Path.Combine(ProfilesDirectory, $"{profileName}.psmgr");

            if (!File.Exists(profilePath))
            {
                MessageBox.Show("Plik profilu nie istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string storedLine = File.ReadLines(profilePath).First();

            // Rozdzielamy metodę szyfrowania i hash hasła
            var parts = storedLine.Split('|');
            if (parts.Length == 2)
            {
                if (Enum.TryParse(parts[0], out EncryptionMethod method))
                {
                    var encryptionManager = new EncryptionManager(EncryptionMethod.SHA256);
                    return encryptionManager.VerifyHash(password, parts[1]);
                }
            }
            else if (parts.Length == 1) // Obsługa starszego formatu pliku bez metody szyfrowania
            {
                var encryptionManager = new EncryptionManager(EncryptionMethod.SHA256);
                return encryptionManager.VerifyHash(password, storedLine);
            }

            MessageBox.Show("Nieprawidłowy format pliku profilu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }



        //private string HashPassword(string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(hashBytes);
        //    }
        //}

    }
}