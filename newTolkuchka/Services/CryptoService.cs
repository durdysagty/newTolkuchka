using newTolkuchka.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace newTolkuchka.Services
{
    public class CryptoService: ICrypto
    {
        const string key = Secrets.key;
        const string iv = Secrets.iv;
        public string EncryptString(string toEncrypt)
        {
            if (toEncrypt == null)
                throw new ArgumentNullException(nameof(toEncrypt));

            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(toEncrypt);
                }
                encrypted = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encrypted);
        }
        public string DecryptString(string value)
        {

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] toDecrypt = Convert.FromBase64String(value);
            string decrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using MemoryStream msDecrypt = new(toDecrypt);
                using CryptoStream csDecrypt = new(msDecrypt, encryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                decrypted = srDecrypt.ReadToEnd();
            }
            return decrypted;
        }
    }
}
