using System.Security.Cryptography;
using System.Text;

namespace VR2.Services
{
    public class CryptoService : ICryptoService
    {
        public (string publicKey, string privateKey) GenerateKeyPair()
        {
            using var rsa = RSA.Create(2048); // إنشاء RSA بحجم 2048 بت
            return (
                publicKey: rsa.ToXmlString(false), // المفتاح العام فقط
                privateKey: rsa.ToXmlString(true)  // المفتاح الخاص مع العام
            );
        }


        public string SignData(string privateKey, string data)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(privateKey); // تحميل المفتاح الخاص

            byte[] dataBytes = Encoding.UTF8.GetBytes(data); // تحويل البيانات لبايتات
            byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1); // التوقيع

            return Convert.ToBase64String(signatureBytes); // تحويل التوقيع إلى string
        }


        public bool VerifySignature(string publicKey, string data, string signature)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
