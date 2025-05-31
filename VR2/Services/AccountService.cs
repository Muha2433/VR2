using Microsoft.AspNetCore.Identity;
using System.Data;
using VR2.DTOqMoels;
using VR2.Models;
using VR2.MyTools;

namespace VR2.Services
{
    public class AccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly FileTools _fileService;
        private readonly ApplicationDbContext _dbContext;
        private readonly TokenService _tokenService;

        public AccountService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            FileTools fileService,
            ApplicationDbContext dbContext,
            TokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _fileService = fileService;
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        //Create Customer
        public async Task<(bool success, string message,string token)> RegisterUserWithImages(DtoUser dtoUser)
        {
            var invalidFiles = new List<string>();

            // Validate user
            var existingUser = await _userManager.FindByEmailAsync(dtoUser.Email);
            if (existingUser != null)
                return (false, "Email already exists",null);
            // Add username uniqueness check
            if (await _userManager.FindByNameAsync(dtoUser.UserName) != null)
                return (false, "Username already exists", null);
            // First pass: Validate all files before processing
            if (dtoUser.lstFileTuple != null && dtoUser.lstFileTuple.Any())
            {
                foreach (var fileTuple in dtoUser.lstFileTuple)
                {
                    foreach (var file in fileTuple.lstRealFile)
                    {
                        if (!_fileService.IsImage(file))
                        {
                            invalidFiles.Add($"{file.FileName} is not a valid image");
                        }
                    }
                }

                // Fail registration if any invalid files found
                if (invalidFiles.Count > 0)
                {
                    return (false, "Invalid image files detected", null);
                }
            }

            // Create new user
            var user = new ModelCustomer
            {
                UserName = dtoUser.UserName,
                Email = dtoUser.Email,
                FirstName = dtoUser.FirstName,
                LastName = dtoUser.LastName,
                Approved = true,
                Rejected = false,
            };


            // Second pass: Save files (we know they're all valid now)
            if (dtoUser.lstFileTuple != null && dtoUser.lstFileTuple.Any())
            {
                var savedFiles = await _fileService.SaveUserFiles(dtoUser.lstFileTuple);

                foreach (var (filePath, fileType) in savedFiles)
                {
                    user.lstPersonalImage_IdentityImage.Add(new ModelFile
                    {
                        fileType = fileType ,
                        ImageName = filePath,
                        AppUserID = user.Id
                    });
                }
            }

            // Create user in transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Create the user
                var result = await _userManager.CreateAsync(user, dtoUser.Password);
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

                // Ensure "Customer" role exists
                const string customerRole = "Customer";
                if (!await _roleManager.RoleExistsAsync(customerRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(customerRole));
                }

                // Assign customer role to the user
                var roleResult = await _userManager.AddToRoleAsync(user, customerRole);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Failed to assign customer role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}", null);
                }
                var token = _tokenService.GenerateJwtToken(user, customerRole);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "User registered successfully as Customer",token);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        // Login Service 

        public async Task<(bool success, string message, string token)> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return (false, "Invalid email or password", null);

            if (!await _userManager.CheckPasswordAsync(user, password))
                return (false, "Invalid email or password", null);
            
            if ((bool)user.Rejected)
                return (false, "Your account has been rejected.", null);

            if ((bool)!user.Approved)
                return (false, "Your account is pending approval.", null);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == null)
                return (false, "User has no assigned role yet", null);

            var token = _tokenService.GenerateJwtToken(user, role);
            return (true, "Login successful", token);
        }


    }
}
