namespace VR2.Services
{
    public interface ICryptoService
    {
        (string publicKey, string privateKey) GenerateKeyPair();
        string SignData(string privateKey, string data);
        bool VerifySignature(string publicKey, string data, string signature);
    }
}
