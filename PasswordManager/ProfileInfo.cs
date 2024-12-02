using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class ProfileInfo
    {
        // Nazwa aktualnie wybranego profilu
        public string ProfileName { get; set; }

        // Ścieżka do pliku profilu
        public string ProfilePath => Path.Combine("Profiles", $"{ProfileName}.psmgr");

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
                // Pierwsza linia: hasło do profilu
                ProfilePassword = lines[0];
            }

            // Pozostałe linie: dane haseł
            Passwords.Clear();
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split('|');
                if (parts.Length == 3) // Zakładamy format: Name|Description|Password
                {
                    Passwords.Add(new PasswordEntry
                    {
                        Name = parts[0],
                        Description = parts[1],
                        Password = parts[2], // Hasło zczytane, ale nie wyświetlane
                        DateAdded = DateTime.Now // Możesz dodać datę, jeśli jest zapisywana
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
                lines.Add($"{entry.Name}|{entry.Description}|{entry.Password}");
            }

            File.WriteAllLines(ProfilePath, lines);
        }

        // Dodaj nowe hasło
        public void AddPassword(string name, string description, string password)
        {
            var entry = new PasswordEntry
            {
                Name = name,
                Description = description,
                Password = password, // Możesz zaszyfrować hasło tutaj, jeśli potrzebujesz
                DateAdded = DateTime.Now
            };

            Passwords.Add(entry);
            SavePasswords(); // Zapisz do pliku
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