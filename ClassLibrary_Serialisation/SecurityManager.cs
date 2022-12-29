using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace ClassLibrary_Serialisation
{
    /// <summary>
    /// Manager to encrypt and decrypt our data with the AES protocol
    /// </summary>
    public static class SecurityManager
    {
        /// <summary>
        /// Encrypt some data with a key
        /// </summary>
        /// <param name="data"> The data to encrypt</param>
        /// <param name="key"> The key used</param>
        /// <returns>Our encrypted data</returns>
        public static byte[] Encrypt(string data, byte[] key)
        {
            // where our encrypted data will be stock
            byte[] encrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
             
                aes.Key = key;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Write the symetric key
                using MemoryStream msEncrypt = new MemoryStream();
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                //Write the data
                ICryptoTransform encoder = aes.CreateEncryptor();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encoder, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(data);
                }
                encrypted = msEncrypt.ToArray();
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypt our encrypted data
        /// </summary>
        /// <param name="cipherText"> the encrypted data</param>
        /// <param name="key"> the key used during the encryption</param>
        /// <returns> The decrypted data </returns>
        public static string Decrypt(byte[] cipherText, byte[] key)
        {
            string decrypted;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                // Initiate our AES decrypter
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] readIV = new byte[16];

                using MemoryStream msDecryptor = new MemoryStream(cipherText);
               
                // Recover our symetric key
                msDecryptor.Read(readIV, 0, 16);
                aes.IV = readIV;
                ICryptoTransform decoder = aes.CreateDecryptor();
               
                // Recover our data
                using CryptoStream csDecryptor = new CryptoStream(msDecryptor, decoder, CryptoStreamMode.Read);
                using StreamReader srReader = new StreamReader(csDecryptor);
                decrypted = srReader.ReadToEnd();
            }
            return decrypted;
        }
    }

}
