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
    /// Logika interakcji dla klasy Ustawienia.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        private MainViewModel _mainViewModel;

        // Konstruktor przyjmujący MainViewModel
        public OptionsView(MainViewModel mainViewModel)
        {
            InitializeComponent();

            if (mainViewModel == null)
            {
                throw new ArgumentNullException(nameof(mainViewModel), "MainViewModel nie został przekazany do OptionsView.");
            }

            _mainViewModel = mainViewModel;

            // Synchronizacja ComboBox z aktualną metodą szyfrowania
            var currentMethod = _mainViewModel.CurrentProfile.EncryptionMethod;
            EncryptionMethodComboBox.SelectedItem = EncryptionMethodComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString().Equals(currentMethod.ToString(), StringComparison.OrdinalIgnoreCase));
        }


        // Obsługa zmiany wyboru w ComboBox
        private void EncryptionMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainViewModel == null)
            {
                return;
            }

            if (EncryptionMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string method = selectedItem.Content.ToString();
                if (Enum.TryParse<EncryptionMethod>(method, out var newMethod))
                {
                    // Sprawdzamy, czy użytkownik faktycznie zmienił metodę
                    if (newMethod != _mainViewModel.CurrentProfile.EncryptionMethod)
                    {
                        var result = MessageBox.Show(
                            $"Czy na pewno chcesz zmienić metodę szyfrowania na {newMethod}?\n" +
                            "Wszystkie hasła zostaną ponownie zaszyfrowane.",
                            "Potwierdzenie zmiany metody",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                _mainViewModel.CurrentProfile.ChangeEncryptionMethod(newMethod);

                                MessageBox.Show(
                                    $"Metoda szyfrowania została zmieniona na {newMethod}.",
                                    "Sukces",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information
                                );
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    $"Błąd podczas zmiany metody szyfrowania: {ex.Message}",
                                    "Błąd",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );

                                // Przywracamy poprzedni wybór w ComboBox
                                EncryptionMethodComboBox.SelectedItem = EncryptionMethodComboBox.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Content.ToString() == _mainViewModel.CurrentProfile.EncryptionMethod.ToString());
                            }
                        }
                        else
                        {
                            // Przywracamy poprzedni wybór w ComboBox
                            EncryptionMethodComboBox.SelectedItem = EncryptionMethodComboBox.Items
                                .Cast<ComboBoxItem>()
                                .FirstOrDefault(i => i.Content.ToString() == _mainViewModel.CurrentProfile.EncryptionMethod.ToString());
                        }
                    }
                }
            }
        }

    }
}