using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy PasswordsView.xaml
    /// </summary>
    public partial class PasswordsView : UserControl
    {
        private ProfileInfo CurrentProfile;
        public PasswordsView()
        {
            InitializeComponent();
            Loaded += PasswordsView_Loaded;
        }

        private void PasswordsView_Loaded(object sender, RoutedEventArgs e)
        {
           
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var mainViewModel = mainWindow.DataContext as MainViewModel;
                if (mainViewModel != null)
                {
                    CurrentProfile = mainViewModel.CurrentProfile;

                  
                    CurrentProfile.LoadPasswords();
                    PasswordsGrid.ItemsSource = CurrentProfile.Passwords;
                }
            }
        }

        private void btnAddPassword_Click(object sender, RoutedEventArgs e)
        {
            var addPasswordWindow = new AddPasswordWindow();
            if (addPasswordWindow.ShowDialog() == true)
            {
              
                CurrentProfile.AddPassword(
                    addPasswordWindow.PasswordName,
                    addPasswordWindow.PasswordDescription,
                    addPasswordWindow.Password // Pobierz hasło
                );

               
                PasswordsGrid.ItemsSource = null;
                PasswordsGrid.ItemsSource = CurrentProfile.Passwords;
            }
        }

        private void PasswordsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEntry = PasswordsGrid.SelectedItem as PasswordEntry;

            if (selectedEntry != null)
            {
                try
                {
                    // Używamy metody szyfrowania z profilu
                    var encryptionManager = new EncryptionManager(CurrentProfile.EncryptionMethod);
                    string decryptedPassword = encryptionManager.Decrypt(selectedEntry.Password, CurrentProfile.ProfilePassword);

                    Clipboard.SetText(decryptedPassword);

                    MessageBox.Show("Skopiowano odszyfrowane hasło. Zostanie usunięte ze schowka po 30 sekundach.",
                        "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);

                    var timer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(30)
                    };

                    timer.Tick += (s, args) =>
                    {
                        Clipboard.Clear();
                        timer.Stop();
                    };

                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas odszyfrowywania hasła: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}