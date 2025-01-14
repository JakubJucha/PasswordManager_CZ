using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logika interakcji dla klasy TestView.xaml
    /// </summary>
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
        }

        
        private void RunPerformanceTest_Click(object sender, RoutedEventArgs e)
        {
            var encryptionManagerAES = new EncryptionManager(EncryptionMethod.AES);
            var encryptionManagerHMAC = new EncryptionManager(EncryptionMethod.HMAC);

            string testData = new string('a', 10_000_000); // 10 MB danych testowych
            string key = "example-key";

            var sw = new Stopwatch();

            // Test AES
            sw.Start();
            var encryptedAES = encryptionManagerAES.Encrypt(testData, key);
            sw.Stop();
            long aesEncryptionTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var decryptedAES = encryptionManagerAES.Decrypt(encryptedAES, key);
            sw.Stop();
            long aesDecryptionTime = sw.ElapsedMilliseconds;

            // Test HMAC
            sw.Restart();
            var encryptedHMAC = encryptionManagerHMAC.Encrypt(testData, key);
            sw.Stop();
            long hmacEncryptionTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var decryptedHMAC = encryptionManagerHMAC.Decrypt(encryptedHMAC, key);
            sw.Stop();
            long hmacDecryptionTime = sw.ElapsedMilliseconds;

           
            ResultsBox.Text = $"Test wydajności szyfrowania:\n" +
                              $"AES - Szyfrowanie: {aesEncryptionTime} ms, Deszyfrowanie: {aesDecryptionTime} ms\n" +
                              $"HMAC - Szyfrowanie: {hmacEncryptionTime} ms, Deszyfrowanie: {hmacDecryptionTime} ms\n";
        }

        

       
        private string BruteForce(EncryptionManager manager, string encryptedPassword, string key, int maxLength, int timeLimitMs)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var sw = new Stopwatch();
            sw.Start();

            foreach (var attempt in GetCombinations(chars, maxLength))
            {
                if (sw.ElapsedMilliseconds > timeLimitMs)
                    return "Przekroczono limit czasu";

                try
                {
                    if (manager.Decrypt(encryptedPassword, key) == attempt)
                    {
                        return attempt;
                    }
                }
                catch
                {
                   
                }
            }

            return ""; 
        }

        
        private IEnumerable<string> GetCombinations(string chars, int maxLength)
        {
            return Enumerable.Range(1, maxLength)
                .SelectMany(length => GenerateCombinations(chars, length));
        }

        private IEnumerable<string> GenerateCombinations(string chars, int length)
        {
            if (length == 0)
                yield return "";
            else
            {
                foreach (var c in chars)
                {
                    foreach (var suffix in GenerateCombinations(chars, length - 1))
                    {
                        yield return c + suffix;
                    }
                }
            }
        }

      
        private void RunPasswordStrengthTest_Click(object sender, RoutedEventArgs e)
        {
            string weakPassword = "123";
            string strongPassword = "1234";

            var encryptionManagerAES = new EncryptionManager(EncryptionMethod.AES);
            var encryptionManagerHMAC = new EncryptionManager(EncryptionMethod.HMAC);

            string encryptedWeakAES = encryptionManagerAES.Encrypt(weakPassword, "key");
            string encryptedStrongAES = encryptionManagerAES.Encrypt(strongPassword, "key");

            string encryptedWeakHMAC = encryptionManagerHMAC.Encrypt(weakPassword, "key");
            string encryptedStrongHMAC = encryptionManagerHMAC.Encrypt(strongPassword, "key");

            ResultsBox.Text += "Test łamania haseł użytkownika:\n";
            ResultsBox.Text += "AES:\n";
            ResultsBox.Text += TestPasswordStrength(encryptionManagerAES, encryptedWeakAES, encryptedStrongAES, "key");
            ResultsBox.Text += "HMAC:\n";
            ResultsBox.Text += TestPasswordStrength(encryptionManagerHMAC, encryptedWeakHMAC, encryptedStrongHMAC, "key");
        }

      
        private string TestPasswordStrength(EncryptionManager manager, string weakPassword, string strongPassword, string key)
        {
            var sw = new Stopwatch();
            string result = "";

            
            sw.Start();
            BruteForce(manager, weakPassword, key, 3, 60000);
            sw.Stop();
            long weakTime = sw.ElapsedMilliseconds;

            
            sw.Restart();
            BruteForce(manager, strongPassword, key, 6, 60000);
            sw.Stop();
            long strongTime = sw.ElapsedMilliseconds;

            result += $"Słabsze hasło: {weakTime} ms, silniejsze hasło: {strongTime} ms\n";
            return result;
        }
    }
}