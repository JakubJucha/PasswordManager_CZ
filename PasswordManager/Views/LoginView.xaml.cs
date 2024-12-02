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

            // Otwórz okno dialogowe
            Application.Current.MainWindow?.Dispatcher.Invoke(() =>
            {
                newProfileWindow.Owner = Application.Current.MainWindow;
            });

            if (newProfileWindow.ShowDialog() == true)
            {
                // Tworzenie pliku profilu
                string profilePath = System.IO.Path.Combine(ProfilesDirectory, $"{newProfileWindow.ProfileName}.psmgr");
                string hashedPassword = HashPassword(newProfileWindow.ProfilePassword);

                // Zapisz zaszyfrowane hasło do pliku
                File.WriteAllText(profilePath, hashedPassword);

                // Dodaj nowy profil do ComboBox
                cbxProfileSelector.Items.Add(newProfileWindow.ProfileName);

                MessageBox.Show($"Profil '{newProfileWindow.ProfileName}' został utworzony.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Załaduj profile do ComboBox
        private void LoadProfiles()
        {
            if (!Directory.Exists(ProfilesDirectory))
            {
                Directory.CreateDirectory(ProfilesDirectory);
            }

            // Wyczyść ComboBox
            cbxProfileSelector.Items.Clear();

            // Pobierz listę profili
            var profileFiles = Directory.GetFiles(ProfilesDirectory, "*.psmgr");
            foreach (var file in profileFiles)
            {
                cbxProfileSelector.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
        }


        // Obsługa przycisku Zaloguj
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

        // Sprawdzenie hasła dla wybranego profilu
        private bool AuthenticateProfile(string profileName, string password)
        {
            string profilePath = System.IO.Path.Combine(ProfilesDirectory, $"{profileName}.psmgr");

            if (!File.Exists(profilePath))
            {
                MessageBox.Show("Plik profilu nie istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Odczytaj zaszyfrowane hasło z pliku
            string storedHash = File.ReadLines(profilePath).First();
            string enteredHash = HashPassword(password);

            // Porównaj hasło z pliku z wpisanym hasłem
            return storedHash == enteredHash;
        }

        // Funkcja do haszowania hasła
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

    }
}