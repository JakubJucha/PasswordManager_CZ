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
        public string RepeatPassword { get; private set; }
        public AddPasswordWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PasswordName = txtName.Text;
            PasswordDescription = txtDescription.Text;
            Password = txtPassword.Password; 
            RepeatPassword = txtConfirmPassword.Password; 

            if (string.IsNullOrEmpty(PasswordName) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Nazwa i hasło są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Password!=RepeatPassword)
            {
                MessageBox.Show("Hasła nie są identyczne.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
         
            DialogResult = false;
            Close();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordStrength != null)
            {
                PasswordStrength.UpdateStrength(txtPassword.Password);
            }
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string generatedPassword = GeneratePassword(16, 20);
            txtPassword.Password = generatedPassword;
            txtConfirmPassword.Password = generatedPassword;
        }

        private string GeneratePassword(int minLength, int maxLength)
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialCharacters = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            const string allCharacters = upperCase + lowerCase + digits + specialCharacters;

            Random random = new Random();
            int passwordLength = random.Next(minLength, maxLength + 1);

            var password = new StringBuilder();
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialCharacters[random.Next(specialCharacters.Length)]);


            for (int i = 4; i < passwordLength; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }


            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }
    }
}
