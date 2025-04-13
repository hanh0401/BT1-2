using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BT1_2
{
    public class CryptoHelper
    {
        // Simple encryption method for demonstration
        public static string Encrypt(string data, string key)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (Aes aes = Aes.Create())
            {
                // Use the first 32 bytes of the key for AES-256
                byte[] truncatedKey = new byte[32];
                Array.Copy(keyBytes.Length >= 32 ? keyBytes : PadKey(keyBytes, 32), truncatedKey, 32);

                // Use the first 16 bytes of the key as IV for simplicity
                byte[] iv = new byte[16];
                Array.Copy(truncatedKey, iv, 16);

                aes.Key = truncatedKey;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encryptedData = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Decrypt(string encryptedData, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (Aes aes = Aes.Create())
            {
                // Use the first 32 bytes of the key for AES-256
                byte[] truncatedKey = new byte[32];
                Array.Copy(keyBytes.Length >= 32 ? keyBytes : PadKey(keyBytes, 32), truncatedKey, 32);

                // Use the first 16 bytes of the key as IV for simplicity
                byte[] iv = new byte[16];
                Array.Copy(truncatedKey, iv, 16);

                aes.Key = truncatedKey;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] decryptedData = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        private static byte[] PadKey(byte[] key, int length)
        {
            byte[] paddedKey = new byte[length];
            for (int i = 0; i < length; i++)
            {
                paddedKey[i] = i < key.Length ? key[i] : (byte)0;
            }
            return paddedKey;
        }

        public static string ComputeHash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateRandomKey(int length)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];
                rng.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }
    }
}
