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
            // Pobierz aktualny profil z MainViewModel
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var mainViewModel = mainWindow.DataContext as MainViewModel;
                if (mainViewModel != null)
                {
                    CurrentProfile = mainViewModel.CurrentProfile;

                    // Załaduj hasła do tabeli
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
                // Dodaj hasło do aktualnego profilu
                CurrentProfile.AddPassword(
                    addPasswordWindow.PasswordName,
                    addPasswordWindow.PasswordDescription,
                    addPasswordWindow.Password // Pobierz hasło
                );

                // Odśwież tabelę
                PasswordsGrid.ItemsSource = null;
                PasswordsGrid.ItemsSource = CurrentProfile.Passwords;
            }
        }

        private void PasswordsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Pobierz wybrany wiersz
            var selectedEntry = PasswordsGrid.SelectedItem as PasswordEntry;

            if (selectedEntry != null)
            {
                // Skopiuj nazwę do schowka (na przykład)
                Clipboard.SetText(selectedEntry.Password);
                MessageBox.Show($"Skopiowano hasło", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}