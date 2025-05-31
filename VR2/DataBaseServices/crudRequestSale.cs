using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using VR2.DTOqMoels;
using VR2.Models;

namespace VR2.DataBaseServices
{
    public class crudRequestSale
    {
        private readonly ApplicationDbContext _dbcontext; 
        public crudRequestSale(ApplicationDbContext dbcontext)
        {
        _dbcontext = dbcontext;
        }
        public async Task AddIndividualRequestForSale(DtoIndividualRequestSale req)
        {
         var share = await _dbcontext.Shares.FindAsync(req.ShareId);
        if (share.IsAvailableForResale == false)
            {

            }
         if (share == null)
             throw new Exception("Share not found when saving request.");


        if (share.RequestID != null)
        {
            throw new Exception("Share added to sale already.");
        }
            var modelRequest = new ModelRequestForSale
        {
        RequestDate = DateTime.Now,
        SaleType = Reqtype.Property.ToString(),
        lstShare = new List<ModelShare> { share },
        TotalAskingPrice = share.SharePrice,
       
        };

         await _dbcontext.RequestForSales.AddAsync(modelRequest);
         await _dbcontext.SaveChangesAsync();
    }


        //public async Task<(bool,string)> AddGroupeRequestForSale(DtoGroupRequestSale req)
        //{

        //        var lstShare = await _dbcontext.Shares
        //                            .Where(s => s.PropertyID == req.PropertyID)
        //                        .ToListAsync();


        //        if (lstShare == null || !lstShare.Any())
        //        {
        //            return (false, "Cannot get list of shares for the property.");

        //    }

        //    // Filter: Only approved shares that are NOT already linked to another sale request
        //    var approvedShares = lstShare
        //        .Where(share =>
        //            req.Approvals.TryGetValue(share.InternalOwnerID, out var approved)
        //            && approved == true
        //            && share.RequestID == null)
        //        .ToList();

        //    if (!approvedShares.Any())
        //    {
        //        return (false, "No owners approved the request.");
        //    }

        //    // Create the sale request
        //    var modelRequest = new ModelRequestForSale
        //    {
        //        RequestDate = DateTime.Now,
        //        SaleType = Reqtype.Property.ToString(),
        //        lstShare = approvedShares,
        //        TotalAskingPrice = approvedShares.Sum(s => s.SharePrice)
        //    };

        //    await _dbcontext.RequestForSales.AddAsync(modelRequest);

        //    // Link each share to the request
        //    foreach (var share in approvedShares)
        //    {
        //        share.RequestID = modelRequest.ID;
        //    }

        //    await _dbcontext.SaveChangesAsync();
        //    return (true, "Group sale request successfully saved.");


        //}

        public async Task<(bool, string)> AddGroupRequestForSale(DtoGroupRequestSale req)
        {
            List<ModelShare> lstShare;

            try
            {
                lstShare = await _dbcontext.Shares
                    .Where(s => s.PropertyID == req.PropertyID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return (false, "Error retrieving shares for the property.");
            }

            if (!lstShare.Any())
            {
                return (false, "Cannot get list of shares for the property.");
            }

            var approvedShares = lstShare
                .Where(share =>
                    req.Approvals.TryGetValue(share.InternalOwnerID, out var approved)
                    && approved == true
                    && share.RequestID == null)
                .ToList();

            if (!approvedShares.Any())
            {
                return (false, "No owners approved the request.");
            }

            var askerShare = lstShare.FirstOrDefault(s => s.ID == req.ShareId);
            approvedShares.Add(askerShare);

            var modelRequest = new ModelRequestForSale
            {
                RequestDate = DateTime.Now,
                SaleType = Reqtype.Property.ToString(),
                lstShare = approvedShares,
                TotalAskingPrice = approvedShares.Sum(s => s.SharePrice)
            };

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                await _dbcontext.RequestForSales.AddAsync(modelRequest);
                await _dbcontext.SaveChangesAsync();
                foreach (var share in approvedShares)
                {
                    share.RequestID = modelRequest.ID;
                    _dbcontext.Entry(share).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Group sale request successfully saved.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine(innerMessage);
                return (false, $"Failed to save the group sale request. Error: {innerMessage}");
            }
        }

   




    }
}
