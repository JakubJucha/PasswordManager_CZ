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
using System.Windows.Shapes;

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy AddPasswordWindow.xaml
    /// </summary>
    public partial class AddPasswordWindow : Window
    {
        public string PasswordName { get; private set; }
        public string PasswordDescription { get; private set; }
        public string Password { get; private set; }
        public AddPasswordWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PasswordName = txtName.Text;
            PasswordDescription = txtDescription.Text;
            Password = txtPassword.Password; // Pobierz hasło z pola

            if (string.IsNullOrEmpty(PasswordName) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Nazwa i hasło są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Anuluj i zamknij okno
            DialogResult = false;
            Close();
        }
    }
}
