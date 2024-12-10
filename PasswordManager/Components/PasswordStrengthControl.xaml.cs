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

namespace PasswordManager.Components
{
    /// <summary>
    /// Logika interakcji dla klasy PasswordStrengthControl.xaml
    /// </summary>
    public partial class PasswordStrengthControl : UserControl
    {
        public PasswordStrengthControl()
        {
            InitializeComponent();
        }

        public void UpdateStrength(string password)
        {
            int score = CalculatePasswordStrength(password);
            StrengthBar.Value = score;

            if (score < 20)
            {
                StrengthText.Text = "Bardzo słabe";
                StrengthBar.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            }
            else if (score < 50)
            {
                StrengthText.Text = "Słabe";
                StrengthBar.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.OrangeRed);
            }
            else if (score < 70)
            {
                StrengthText.Text = "Dostateczne";
                StrengthBar.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
            }
            else if (score < 100)
            {
                StrengthText.Text = "Dobre";
                StrengthBar.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.YellowGreen);
            }
            else
            {
                StrengthText.Text = "Bardzo dobre";
                StrengthBar.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            }
        }

        private int CalculatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;


            if (password.Length >= 8) score += 25;  
            if (password.Length >= 12) score += 35; 
            if (password.Length >= 16) score += 40;

            bool hasLower = System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]");
            bool hasUpper = System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]");
            bool hasDigit = System.Text.RegularExpressions.Regex.IsMatch(password, @"\d");
            bool hasSpecial = System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#\$%\^&\*\(\)_\+\-=\[\]\{\};:'"",<>\.\?/\\|`~]");

            if (hasLower) score += 10;
            if (hasUpper) score += 15;
            if (hasDigit) score += 15;
            if (hasSpecial) score += 20;

            int uniqueChars = password.Distinct().Count();
            if (uniqueChars >= 6 && uniqueChars <= 10) score += 10;
            if (uniqueChars > 10) score += 20;

            if (!(password.Length >= 12 && hasSpecial && hasDigit) && score > 80)
            {
                score = 80;
            }

            return Math.Clamp(score, 0, 100);
        }
    }
}
