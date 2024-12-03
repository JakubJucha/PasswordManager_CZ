using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace PasswordManager
{
    public class ProfileInfo
    {
        // Nazwa aktualnie wybranego profilu
        public string ProfileName { get; set; }

        public static readonly string ProfilesDirectory = "Profiles";
        // Ścieżka do pliku profilu
        public string ProfilePath => Path.Combine(ProfilesDirectory, $"{ProfileName}.psmgr");


        // Opcjonalne ustawienia związane z profilem
        public string EncryptionMethod { get; set; } = "SHA256";

        // Lista zapisanych haseł
        public List<PasswordEntry> Passwords { get; private set; } = new List<PasswordEntry>();

        // Hasło do profilu (przechowywane w pierwszej linii pliku)
        public string ProfilePassword { get; private set; }

        // Wczytaj dane z pliku
        public void LoadPasswords()
        {
            if (!File.Exists(ProfilePath))
            {
                Passwords.Clear();
                ProfilePassword = string.Empty;
                return;
            }

            var lines = File.ReadAllLines(ProfilePath);

            if (lines.Length > 0)
            {
                ProfilePassword = lines[0]; // Pierwsza linia: hasło profilu
            }

            Passwords.Clear();
            var encryptionManager = new EncryptionManager(PasswordManager.EncryptionMethod.AES);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split('|');
                if (parts.Length == 4) // Zakładamy format: Name|Description|IV|CipherText
                {
                    Passwords.Add(new PasswordEntry
                    {
                        Name = parts[0],
                        Description = parts[1],
                        Password = $"{parts[2]}|{parts[3]}", // Trzymamy zaszyfrowane hasło
                        DateAdded = DateTime.Now // Data nie jest odczytywana z pliku, możesz to zmienić
                    });
                }
            }
        }


        // Zapisz dane do pliku
        public void SavePasswords()
        {
            var lines = new List<string>
    {
        ProfilePassword // Pierwsza linia: hasło profilu
    };

            foreach (var entry in Passwords)
            {
                // Upewnij się, że hasło jest już zaszyfrowane
                lines.Add(entry.Password.Contains('|')
                    ? $"{entry.Name}|{entry.Description}|{entry.Password}" // Zapisujemy zaszyfrowane dane w oryginalnym formacie
                    : throw new InvalidOperationException("Hasło nie jest zaszyfrowane przed zapisem."));
            }

            File.WriteAllLines(ProfilePath, lines);
        }

        public void AddPassword(string name, string description, string plainPassword)
        {
            var encryptionManager = new EncryptionManager(PasswordManager.EncryptionMethod.AES);
            string encryptedPassword = encryptionManager.Encrypt(plainPassword, ProfilePassword);

            var entry = new PasswordEntry
            {
                Name = name,
                Description = description,
                Password = encryptedPassword, // Trzymamy zaszyfrowaną formę w pamięci
                DateAdded = DateTime.Now
            };

            Passwords.Add(entry);
            SavePasswords();
        }

        // Usuń hasło
        public void RemovePassword(PasswordEntry entry)
        {
            Passwords.Remove(entry);
            SavePasswords();
        }
    }

    // Klasa reprezentująca pojedyncze hasło
    public class PasswordEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public DateTime DateAdded { get; set; }
    }
}