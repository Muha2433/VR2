using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Transactions;
using VR2.Models;

namespace VR2.Services
{
    public class PurchaseService
    {
        private readonly ApplicationDbContext _dbContext;

        public PurchaseService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class BuyerPercentage
        {
            public ModelCustomer Buyer { get; set; }
            public double Percentage { get; set; }
        }
        public async Task CreatePurchase(ModelPurchase purChase,List<string> password , int RequestSaleID )
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var requestSale = await _dbContext.RequestForSales
                .Include(r => r.lstShare)
                .ThenInclude(s => s.InternalOwner)
                .FirstOrDefaultAsync(r => r.ID == RequestSaleID);

                if (requestSale == null)
                    throw new Exception("Sale request not found.");

                List<BuyerPercentage> lstBuyerwithPercentage = purChase.lstPurchaseCustomer
            .Select(pc => new BuyerPercentage
          {
              Buyer = pc.Buyer,
              Percentage = pc.Percentage
         }).ToList();

                var lstSellers = requestSale.lstShare
                .Select(s => s.InternalOwner)
                .Distinct() // Ensure no duplicate sellers
                .ToList();

                List<ModelShare> lstShare = requestSale.lstShare.ToList();

               await TransferOwnerShip(lstShare, lstBuyerwithPercentage, lstSellers);



            }
            catch 
            {
            transaction.Rollback();
            }
            
            //Chat Gpt Write accurate code 
            //beginTransaction
            //GenerateContract
            //TransferOwnerShip
            //endTransaction
        }
        public async Task GenerateContract(List<BuyerPercentage> lstBuyerwithPercentage, List<ModelShare> lstShare)
        {
            //Chat Gpt Write accurate code
            
        }
        public async Task TransferOwnerShip(List<ModelShare> lstShare, 
            List<BuyerPercentage> lstBuyerWithPercentage, List<ModelCustomer> lstSellers)
        {
            //Chat Gpt Write accurate code 
            List<ModelShare> lstNewShares = new List<ModelShare>();

            // Step 1: Calculate total original seller percentage (e.g., 100%)
            double totalSellerOwnership = lstShare.Sum(s => s.SharePercentageOfWholePropert);

            double totalSellerSharePrice = (double)lstShare.Sum(s => s.PurchasePrice);
            
            // Optional validation
            if (totalSellerOwnership <= 0 || totalSellerSharePrice<=0)
                throw new Exception("Total seller ownership and total SharePrice  must be greater than 0.");

            // Step 2: For each buyer, create new share based on percentage of the full property
            int? propertyID = lstShare.FirstOrDefault()?.PropertyID;
            if (propertyID == null)
                throw new Exception("error in property referenced");
           var referencedProperty=await _dbContext.Properties.FindAsync(propertyID);
            foreach (var BuyerPercentage in lstBuyerWithPercentage)
            {

                ModelShare share = new ModelShare()
                {
                    PropertyID = propertyID.Value,
                    Property= referencedProperty,
                    InternalOwnerID=BuyerPercentage.Buyer.Id,
                    InternalOwner=BuyerPercentage.Buyer,
                    SharePercentageOfWholePropert=totalSellerOwnership*BuyerPercentage.Percentage,
                    IsExternalOwner=false,
                    SharePrice=totalSellerSharePrice* BuyerPercentage.Percentage,
                };

                lstNewShares.Add(share);
            }
           // Remove seller's shares from lstShare if they paid for it
             foreach (var seller in lstSellers)
            {
                var customer = await _dbContext.Customers.Include(c=> c.lstShare).FirstOrDefaultAsync(c => c.Id == seller.Id);
                if (customer == null)
                    throw new Exception("Seller Must Have Share");

                foreach (var share in lstShare.ToList()) // To avoid modifying the collection while iterating
                {
                    if (seller.lstShare.Contains(share)) // Check if the seller has this share
                    {
                       
                      
                         customer.lstShare.Remove(share);  // Remove the share from the seller's lstShare collection
                        
                        _dbContext.Shares.Remove(share);  // Also remove the share from the database
                    }
                }
            }

            // Step 3: Save changes
            _dbContext.Shares.RemoveRange(lstShare);

            _dbContext.Shares.AddRangeAsync(lstNewShares);
     
            await _dbContext.SaveChangesAsync();

        }


    }
    

}

