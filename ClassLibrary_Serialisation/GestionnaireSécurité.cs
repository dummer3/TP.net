using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace ClassLibrary_Serialisation
{
    public static class GestionnaireSécurité
    {

        public static byte[] Encrypt(string str, byte[] keys)
        {
            byte[] encrypted;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keys;

                aes.GenerateIV();

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using MemoryStream msEncrypt = new MemoryStream();
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                ICryptoTransform encoder = aes.CreateEncryptor();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encoder, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(str);
                }
                encrypted = msEncrypt.ToArray();
            }

            return encrypted;
        }

        public static string Decrypt(byte[] cipherText, byte[] key)
        {
            string decrypted;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecryptor = new MemoryStream(cipherText))
                {
                    byte[] readIV = new byte[16];
                    msDecryptor.Read(readIV, 0, 16);
                    aes.IV = readIV;
                    ICryptoTransform decoder = aes.CreateDecryptor();
                    using CryptoStream csDecryptor = new CryptoStream(msDecryptor, decoder, CryptoStreamMode.Read);
                    using StreamReader srReader = new StreamReader(csDecryptor);
                    decrypted = srReader.ReadToEnd();
                }
            }
            return decrypted;
        }
    }

}
