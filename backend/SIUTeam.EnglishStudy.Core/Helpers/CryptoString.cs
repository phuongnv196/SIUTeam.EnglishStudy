using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SIUTeam.EnglishStudy.Core.Helpers
{
    public class CryptoString(IConfiguration configuration)
    {
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            var key = configuration["EncryptionKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(key, "Encryption key must be provided.");
            }
            aes.Key = GetKeyBytes(key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV vào đầu

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string encryptedText)
        {
            var key = configuration["EncryptionKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(key, "Encryption key must be provided.");
            }
            var fullCipher = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = GetKeyBytes(key);

            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
        private static byte[] GetKeyBytes(string key)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }
}
