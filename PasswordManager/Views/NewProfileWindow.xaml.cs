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
using System.Windows.Shapes;

namespace PasswordManager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy NewProfileWindow.xaml
    /// </summary>
    public partial class NewProfileWindow : Window
    {
        public string ProfileName { get; private set; }
        public string ProfilePassword { get; private set; }

        public NewProfileWindow()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string name = txtProfileName.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Podaj nazwę profilu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Hasło nie może być puste.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Hasła nie zgadzają się.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Zapisz dane profilu
            ProfileName = name;
            ProfilePassword = password;

            // Utwórz plik profilu
            SaveProfileFile(name, password);

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveProfileFile(string name, string password)
        {
            string profilePath = $"{name}.psmgr";
            string encryptedPassword = EncryptPassword(password);

            File.WriteAllText(profilePath, encryptedPassword);
        }

        private string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}