using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace FIA.SME.Aquisicao.Core.Helpers
{
    public interface ITextEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }

    internal class AesTextEncryptor : ITextEncryptor
    {
        private readonly IConfiguration _configuration;

        public AesTextEncryptor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encrypt(string text)
        {
            CheckParam(text);

            var secret = GetSecret(_configuration);

            var salt = Encoding.ASCII.GetBytes(secret.Length.ToString());

            var aesCipher = Aes.Create("AesManaged");
            var plainText = Encoding.Unicode.GetBytes(text);
            var SecretKey = new PasswordDeriveBytes(secret, salt);

            using (var encryptor = aesCipher!.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainText, 0, plainText.Length);
                cryptoStream.FlushFinalBlock();
                text = Convert.ToBase64String(memoryStream.ToArray());
            }

            return text;
        }

        public string Decrypt(string text)
        {
            CheckParam(text);

            var secret = GetSecret(_configuration);

            var salt = Encoding.ASCII.GetBytes(secret.Length.ToString());

            var aesCipher = Aes.Create("AesManaged");
            var encryptedData = Convert.FromBase64String(text);
            var secretKey = new PasswordDeriveBytes(secret, salt);

            using (var decryptor = aesCipher!.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (var memoryStream = new MemoryStream(encryptedData))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                var plainText = new byte[encryptedData.Length];
                var decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                text = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
            }

            return text;
        }

        private static string GetSecret(IConfiguration configuration)
        {
            return configuration.GetSection("Jwt:CryptographySecret").Value;
        }

        private static void CheckParam(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));
        }
    }
}
