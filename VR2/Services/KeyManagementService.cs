using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using VR2.Models;
using VR2.MyTools;

namespace VR2.Services
{
    public class KeyManagementService : IKeyManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICryptoService _cryptoService;
        private readonly UserManager<AppUser> _userManager;
        public KeyManagementService(ApplicationDbContext context,
            ICryptoService cryptoService , UserManager<AppUser> userManager)
        {
            _context = context;
            _cryptoService = cryptoService;
            _userManager = userManager;
        }

        public async Task GenerateAndStoreKeysForUser(string userId, string password)
        {
            var (publicKey, privateKey) = _cryptoService.GenerateKeyPair();

            // Encrypt private key with user's password
            string encryptedPrivateKey = EncryptionTools.Encrypt(privateKey, password);
            var user =  await _userManager.FindByIdAsync(userId);
            user.PublicKey=publicKey;
            user.PrivateKey=encryptedPrivateKey;

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChangesAsync();

        }

        public async Task<string> GetPrivateKey(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.PrivateKey))
                return null;

            // ✅ Check if the entered password matches the stored hash
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
                return null;

            try
            {
                // ✅ Password is correct, now decrypt the private key
                string decryptedPrivateKey = EncryptionTools.Decrypt(user.PrivateKey, password);
                return decryptedPrivateKey;
            }
            catch (CryptographicException)
            {
                // Something went wrong during decryption
                return null;
            }
        }

    }

}
