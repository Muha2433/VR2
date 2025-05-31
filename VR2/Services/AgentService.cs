using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VR2.DTOqMoels;
using VR2.Models;

namespace VR2.Services
{
    public class AgentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AgentService(ApplicationDbContext dbContext,
                            UserManager<AppUser> userManager,
                            RoleManager<IdentityRole> roleManager) 
        {
        _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<(bool, string)> AcceptOrRejectAsync(AgentDecisionRequestDto dto)
        {
            if (dto.PropertyID == 0 || string.IsNullOrWhiteSpace(dto.Decision) ||
                (dto.Decision.ToLower() != "accept" && dto.Decision.ToLower() != "reject"))
            {
                return (false, "Property ID was invalid or decision was undefined.");
            }

            var property = await _dbContext.Properties.FindAsync(dto.PropertyID);
            if (property == null)
            {
                return (false, "Property not found.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            if (dto.Decision.ToLower() == "reject")
            {
                property.Rejected = true;
                property.Accepted = false;

                _dbContext.Entry(property).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Property rejected successfully.");
            }

            // Accept logic
            property.Rejected = false;
            property.Accepted = true;

            var (success, message) = await ProcessSharesAsync(property, dto.lstShares);
            if (!success)
            {
                return (false, message);
            }
            _dbContext.Entry(property).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            // _dbContext.Entry(property).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return(false, "Save error: " + inner);
            }

            //await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Property accepted and shares assigned successfully.");
        }



        private async Task<(bool success, string message)> ProcessSharesAsync(ModelProperties property, ICollection<DtoShare> shares)
        {
            if (shares == null || !shares.Any())
                return (false, "Please add shares for the property.");

            var totalPercentage = shares.Sum(s => s.SharePercentageOfWholePropert);
            if (totalPercentage > 100)
                return (false, "Total share percentage cannot exceed 100%.");

            foreach (var share in shares)
            {
                var newShare = new ModelShare
                {
                    SharePercentageOfWholePropert = share.SharePercentageOfWholePropert,
                    SharePrice = (share.SharePercentageOfWholePropert / 100.0) * property.Price,
                    PurchasePrice = null,
                    IsAvailableForResale = false,
                    PropertyID = property.ID,
                    IsExternalOwner = share.IsExternalOwner ?? false
                };

                if (newShare.IsExternalOwner)
                {
                    newShare.ExternalOwnerID = share.External_Seller_ID;
                }
                else
                {
                    var internalCustomer = (ModelCustomer)await _dbContext.Customers.FindAsync(share.Internal_Seller_ID);
                    if (internalCustomer == null || !await _userManager.IsInRoleAsync(internalCustomer, "Customer"))
                    {
                        return (false, $"Internal seller with ID {share.Internal_Seller_ID} is not a valid customer.");
                    }

                    newShare.InternalOwnerID = internalCustomer.Id;
                    // Do NOT set InternalOwner object manually
                }

                _dbContext.Shares.Add(newShare);
            }

            return (true, null);
        }


        public async Task<(bool,List<DtoUsernameID>,string)> GetListOfUname() 
        {
            try
            {
                // Get all users
                var allUsers = _userManager.Users.ToList();

                var customers = new List<DtoUsernameID>();

                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        customers.Add(new DtoUsernameID
                        {
                            ID = user.Id,
                            UName = user.UserName
                        });
                    }
                }

                return (true, customers, "Success");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error: {ex.Message}");
            }
        }


        public async Task<(bool, List<DtoUsernameID>, string)> EfficientGetListOfUname()
        {
            try
            {
                // Get role ID for "Customer"
                var customerRole = await _roleManager.FindByNameAsync("Customer");
                if (customerRole == null)
                    return (false, null, "Customer role not found");

                // Join AspNetUserRoles and AspNetUsers to get users with the Customer role
                var customerUsers = await (from user in _dbContext.Users
                                           join userRole in _dbContext.UserRoles
                                               on user.Id equals userRole.UserId
                                           where userRole.RoleId == customerRole.Id
                                           select new DtoUsernameID
                                           {
                                               ID = user.Id,
                                               UName = user.UserName
                                           }).ToListAsync();

                return (true, customerUsers, "Success");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error: {ex.Message}");
            }
        }


    

    }
}
