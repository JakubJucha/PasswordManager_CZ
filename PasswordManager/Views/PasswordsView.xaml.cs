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
        private System.Windows.Threading.DispatcherTimer activeTimer = null;

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
                    // Zatrzymaj aktywny licznik (jeśli istnieje)
                    if (activeTimer != null)
                    {
                        activeTimer.Stop();
                        activeTimer = null;
                    }

                    // Używamy metody szyfrowania z profilu
                    var encryptionManager = new EncryptionManager(CurrentProfile.EncryptionMethod);
                    string decryptedPassword = encryptionManager.Decrypt(selectedEntry.Password, CurrentProfile.ProfilePassword);

                    Clipboard.SetText(decryptedPassword);

                    StartClipboardCountdown("Hasło zostało skopiowane do schowka.");

                    btnDeletePassword.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    lblClipboardInfo.Content = $"Błąd: {ex.Message}";
                    progressClipboard.Visibility = Visibility.Collapsed;
                    btnDeletePassword.IsEnabled = false;
                }
            }
            else
            {
                // Wyłącz przycisk usuwania, jeśli nic nie jest wybrane
                btnDeletePassword.IsEnabled = false;
            }
        }

        private void StartClipboardCountdown(string message)
        {
            const int countdownTime = 30; // 30 sekund
            int remainingTime = countdownTime;

            // Zatrzymaj aktywny licznik, jeśli istnieje
            if (activeTimer != null)
            {
                activeTimer.Stop();
                activeTimer = null;
            }

            lblClipboardInfo.Content = $"{message} Zostanie usunięte za {remainingTime} sekund.";
            progressClipboard.Value = countdownTime;
            progressClipboard.Visibility = Visibility.Visible;

            // Tworzenie nowego licznika
            activeTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            activeTimer.Tick += (s, args) =>
            {
                remainingTime--;
                lblClipboardInfo.Content = $"{message} Zostanie usunięte za {remainingTime} sekund.";
                progressClipboard.Value = remainingTime;

                if (remainingTime <= 0)
                {
                    Clipboard.Clear();
                    lblClipboardInfo.Content = "Schowek został wyczyszczony.";
                    progressClipboard.Visibility = Visibility.Collapsed;
                    activeTimer.Stop();
                    activeTimer = null; // Zresetuj licznik
                }
            };

            activeTimer.Start();
        }


        private void btnDeletePassword_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = PasswordsGrid.SelectedItem as PasswordEntry;

            if (selectedEntry == null)
            {
                MessageBox.Show("Nie wybrano hasła do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć hasło '{selectedEntry.Name}'?",
                "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Usuń aktywny licznik (jeśli istnieje)
                    if (activeTimer != null)
                    {
                        activeTimer.Stop();
                        activeTimer = null;
                    }

                    // Usuń hasło z profilu
                    CurrentProfile.RemovePassword(selectedEntry);

                    // Odśwież tabelkę
                    PasswordsGrid.ItemsSource = null;
                    PasswordsGrid.ItemsSource = CurrentProfile.Passwords;

                    // Wyczyszczenie schowka i reset widoku
                    Clipboard.Clear();
                    lblClipboardInfo.Content = "Wybierz hasło, aby je skopiować";
                    progressClipboard.Visibility = Visibility.Collapsed;

                    btnDeletePassword.IsEnabled = false;

                    MessageBox.Show($"Hasło '{selectedEntry.Name}' zostało usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania hasła: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}