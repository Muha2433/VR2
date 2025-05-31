namespace VR2.Services
{
    public interface IKeyManagementService
    {
        Task GenerateAndStoreKeysForUser(string userId, string password);
        Task<string> GetPrivateKey(string userId, string password);
    }
}
