using System.Security.Cryptography;
using System.Text;

namespace VR2.MyTools
{
    public static class EncryptionTools
    {
        public static string Encrypt(string plainText, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] iv = RandomNumberGenerator.GetBytes(16);

            var key = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256)
                .GetBytes(32);   // 256-bit AES

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);


            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);
            ms.Write(iv, 0, iv.Length);
            ms.Write(encryptedBytes, 0, encryptedBytes.Length);

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            var fullBytes = Convert.FromBase64String(cipherText);

            byte [] salt = new byte[16];
            byte [] iv = new byte[16];
            byte [] cipherBytes=new byte[fullBytes.Length - salt.Length - iv.Length];

            //
            Array.Copy(fullBytes, 0, salt, 0, 16);
            Array.Copy(fullBytes, 16, iv, 0, 16);
            Array.Copy(fullBytes,32,cipherBytes, 0, cipherBytes.Length);
            //

            var key = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256).GetBytes(32); ;

            //

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            //

            using var decriptor= aes.CreateDecryptor();

            byte[] plainByte = decriptor.TransformFinalBlock(cipherBytes,0,cipherBytes.Length);

            return Encoding.UTF8.GetString(plainByte);

        }

    }

}
