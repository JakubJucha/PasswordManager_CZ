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
        public EncryptionMethod EncryptionMethod { get; set; } = EncryptionMethod.AES; 

        public string ProfileName { get; set; }

        public static readonly string ProfilesDirectory = "Profiles";
    
        public string ProfilePath => Path.Combine(ProfilesDirectory, $"{ProfileName}.psmgr");


        public List<PasswordEntry> Passwords { get; private set; } = new List<PasswordEntry>();

       
        public string ProfilePassword { get; private set; }


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
                var firstLine = lines[0].Split('|');
                if (firstLine.Length == 2 && Enum.TryParse(firstLine[0], out EncryptionMethod method))
                {
                    EncryptionMethod = method; // Odczytujemy metodę szyfrowania
                    ProfilePassword = firstLine[1]; // Odczytujemy hash hasła
                }
                else if (firstLine.Length == 1) // Obsługa starszych plików
                {
                    EncryptionMethod = EncryptionMethod.AES; // Domyślnie AES
                    ProfilePassword = firstLine[0];
                }
            }

            Passwords.Clear();
            var encryptionManager = new EncryptionManager(EncryptionMethod);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split('|');
                if (parts.Length == 4) //Name|Description|IV|CipherText
                {
                    Passwords.Add(new PasswordEntry
                    {
                        Name = parts[0],
                        Description = parts[1],
                        Password = $"{parts[2]}|{parts[3]}",
                        DateAdded = DateTime.Now
                    });
                }
            }
        }




        public void SavePasswords()
        {
            var lines = new List<string>
    {
        $"{EncryptionMethod}|{ProfilePassword}" // Zapisujemy metodę szyfrowania obok hash-a
    };

            foreach (var entry in Passwords)
            {
                lines.Add(entry.Password.Contains('|')
                    ? $"{entry.Name}|{entry.Description}|{entry.Password}"
                    : throw new InvalidOperationException("Hasło nie jest zaszyfrowane przed zapisem."));
            }

            File.WriteAllLines(ProfilePath, lines);
        }


        public void AddPassword(string name, string description, string plainPassword)
        {
            var encryptionManager = new EncryptionManager(EncryptionMethod); 
            string encryptedPassword = encryptionManager.Encrypt(plainPassword, ProfilePassword);

            var entry = new PasswordEntry
            {
                Name = name,
                Description = description,
                Password = encryptedPassword, 
                DateAdded = DateTime.Now
            };

            Passwords.Add(entry);
            SavePasswords();
        }



        public void RemovePassword(PasswordEntry entry)
        {
            Passwords.Remove(entry);
            SavePasswords();
        }


        public void ChangeEncryptionMethod(EncryptionMethod newMethod)
        {
            // Odszyfruj wszystkie hasła starą metodą
            var oldEncryptionManager = new EncryptionManager(EncryptionMethod);
            var newEncryptionManager = new EncryptionManager(newMethod);

            foreach (var entry in Passwords)
            {
                // Odszyfruj hasło starą metodą
                string decryptedPassword = oldEncryptionManager.Decrypt(entry.Password, ProfilePassword);

                // Zaszyfruj hasło nową metodą
                entry.Password = newEncryptionManager.Encrypt(decryptedPassword, ProfilePassword);
            }

            // Zmień metodę szyfrowania w profilu
            EncryptionMethod = newMethod;

            // Zapisz zaktualizowane dane
            SavePasswords();
        }



    }

    public class PasswordEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public DateTime DateAdded { get; set; }
    }
}