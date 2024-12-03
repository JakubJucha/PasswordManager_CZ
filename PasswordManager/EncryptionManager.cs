using System;
using System.Security.Cryptography;
using System.Text;


namespace PasswordManager
{
    public enum EncryptionMethod
    {
        SHA256,
        AES 
    }

    public class EncryptionManager
    {
        private readonly EncryptionMethod _method;

        public EncryptionManager(EncryptionMethod method)
        {
            _method = method;
        }

       
        public string Hash(string input)
        {
            switch (_method)
            {
                case EncryptionMethod.SHA256:
                    return HashSHA256(input);
                default:
                    throw new NotSupportedException($"Metoda szyfrowania {_method} nie jest obsługiwana.");
            }
        }

     
        public bool VerifyHash(string input, string hash)
        {
            return Hash(input) == hash;
        }

      
        public string Encrypt(string input, string key)
        {
            switch (_method)
            {
                case EncryptionMethod.AES:
                    return EncryptAES(input, key);
                default:
                    throw new NotSupportedException($"Metoda szyfrowania {_method} nie jest obsługiwana.");
            }
        }

      
        public string Decrypt(string encryptedInput, string key)
        {
            switch (_method)
            {
                case EncryptionMethod.AES:
                    return DecryptAES(encryptedInput, key);
                default:
                    throw new NotSupportedException($"Metoda szyfrowania {_method} nie jest obsługiwana.");
            }
        }

      
        private string HashSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hashBytes);
            }
        }

      
        private string EncryptAES(string plainText, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = GenerateKey(key); // Upewnij się, że klucz ma odpowiednią długość
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherTextBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                string ivBase64 = Convert.ToBase64String(iv);
                string cipherTextBase64 = Convert.ToBase64String(cipherTextBytes);

                return $"{ivBase64}|{cipherTextBase64}";
            }
        }

    
        private string DecryptAES(string encryptedInput, string key)
        {
            var parts = encryptedInput.Split('|');
            if (parts.Length != 2)
            {
                throw new FormatException("Niepoprawny format zaszyfrowanych danych.");
            }

            string ivBase64 = parts[0];
            string cipherTextBase64 = parts[1];

            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            using (var aes = Aes.Create())
            {
                aes.Key = GenerateKey(key); 
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] plainBytes = decryptor.TransformFinalBlock(cipherTextBytes, 0, cipherTextBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }

        private byte[] GenerateKey(string key)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}