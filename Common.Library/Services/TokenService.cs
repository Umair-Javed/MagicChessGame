using Common.Library.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Services
{
    // Represents a service for token encryption and decryption.
    public class TokenService : ITokenService
    {
        #region Constants

        // Replace with a strong and secret password
        private const string Key = "yourSecretKey12345678901234567890";

        // Replace with a unique salt
        private const string Salt = "yourSalt1234567890";

        #endregion

        #region Token Encryption

        // Encrypts the provided data into a token.
        public string EncryptToken(object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKey();
                aesAlg.IV = new byte[16]; // You might want to generate a random IV for better security

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(jsonData);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        #endregion

        #region Token Decryption

        // Decrypts the provided token into the specified type.
        public T DecryptToken<T>(string encryptedData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKey();
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string jsonData = srDecrypt.ReadToEnd();
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
                        }
                    }
                }
            }
        }

        #endregion

        #region Key Derivation

        // Derives a key for encryption and decryption.
        private byte[] DeriveKey()
        {
            using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(Key, Encoding.UTF8.GetBytes(Salt), 10000, HashAlgorithmName.SHA256))
            {
                return keyDerivationFunction.GetBytes(32); // 256 bits
            }
        }

        #endregion
    }
}
