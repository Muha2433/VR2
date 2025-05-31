using Microsoft.EntityFrameworkCore;
using VR2.DTOqMoels;
using VR2.Models;

namespace VR2.Services
{
    public class CustomerServices
    {

        private readonly ApplicationDbContext _dbContext;
        public CustomerServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool,string,ModelCustomer)> GetCustomerByID(string id)
        {
            if (id == null) return (false, "id null", null);
            var cuto = await _dbContext.Customers
    .Include(c => c.lstPersonalImage_IdentityImage)
    .FirstOrDefaultAsync(c => c.Id == id);
            if (cuto == null) return (false, "customer doesnot exist", null); ;
            return (true,"succssfully",cuto);
        }
        public async Task<(bool, string, List<ModelShare>)> GetListOfShare(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                return (false, "ID Customer not appropertiate.", null);

            }
            string customerId = ID;

            var customer = await _dbContext.Customers
                .Include(c => c.lstShare)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return (false, "Customer not found.", null);
            }
            var lstShare = new List<ModelShare>();
            foreach (var item in customer.lstShare)
            {
                lstShare.Add(item);
            }
            if (lstShare == null || !lstShare.Any())
            {
                return (true, "Customer has no share now", null);

            }
            return (true, "Customer has the following shares  now", lstShare);

        }



        public async Task<(bool,string,List<DtoUsernameID>)> GetOwnerOfProperty(int IdOfShare)
        {
            if (IdOfShare==null||IdOfShare < 0) 
            {
                return (false, "ID of share are not correct ", null);
            }
            var share=await _dbContext.Shares.FindAsync(IdOfShare);
            if (share == null)
            {
                return (false,"share doenot exist",null);
            }
            var property = await _dbContext.Properties.Include(lstShare => lstShare.lstShare).
                FirstOrDefaultAsync(p=>p.ID==share.PropertyID);
            if (property == null)
            {
                return (false, "property doenot exist", null);
            }
            var lstOwner=new List<DtoUsernameID>   ();
            foreach (var item in property.lstShare)
            {
            var customer=await _dbContext.Customers.FindAsync(item.InternalOwnerID);
                if (customer == null) {
                    return (false, "customer doenot exist", null);
                }
                lstOwner.Add(new DtoUsernameID { ID = customer.Id ,UName=customer.UserName});
            }
            if (!lstOwner.Any()) 
            {
                return (false, "owners doenot exist", null);
            }
            return (true, "owners returned successfully",lstOwner);

        }

        public async Task<string> GetUserName(string userId)
        {
           var cuto= await _dbContext.Customers.FindAsync(userId);
           if (cuto == null) return null;
            return cuto.UserName;
        }

    }
}
