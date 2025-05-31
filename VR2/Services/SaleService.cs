using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VR2.DTOqMoels;
using VR2.Models;

namespace VR2.Services
{
    public class SaleService
    {
        private readonly ApplicationDbContext _dbContext;

        public SaleService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<(List<string> lstImage, string type,
            string Country_City, double price, int number_bedroom, 
            int number_bathrooms, double percantageOwnerShip)>> ReqSalelist()
        {
            var results = await _dbContext.RequestForSales
               // .Where(r => !r.Processed) // Filter for unprocessed requests if needed
                .Include(e => e.lstShare)
                    .ThenInclude(s => s.Property)
                        .ThenInclude(p => p.Location)
                .Include(e => e.lstShare)
                    .ThenInclude(s => s.Property)
                        .ThenInclude(p => p.lstPropertiesImage_Document)
                .Select(r => new
                {
                    Property = r.lstShare.FirstOrDefault().Property,
                    Share = r.lstShare.FirstOrDefault(),
                    Images = r.lstShare.FirstOrDefault().Property.lstPropertiesImage_Document
                                .Where(f => f.fileType == "PropertyImages" && f.ImageName != null)
                                .Select(f => f.ImageName)
                                .ToList()
                })
                .ToListAsync();

            return results.Select(x => (
                lstImage: x.Images ?? new List<string>(),
                type: "Resale",
                Country_City: x.Property.Location != null
                    ? $"{x.Property.Location.Country}/{x.Property.Location.City}"
                    : "Unknown/Unknown",
                price: x.Share?.SharePrice ?? 0,
                number_bedroom: x.Property.Bedrooms,
                number_bathrooms: x.Property.Bathrooms,
                percantageOwnerShip: x.Share?.SharePercentageOfWholePropert ?? 0
            )).ToList();
        }
        //////////////////////////////////////
        ///
        public async Task<List<dtoPropertyReuestSale>> RequestPropertySalellist()
        {
            var result = await _dbContext.Shares
    .Where(s => s.RequestID != null)
    .GroupBy(s => s.PropertyID)
    .Select(g => new dtoPropertyReuestSale
    {
    
        IdOfRequest = g.First().RequestID.Value,
        // Get basic property info
        numberOfBedRooms = g.First().Property.Bedrooms,
        numberOfBathrooms = g.First().Property.Bathrooms,

        // Sum share percentages and prices
        percantageOfWholeProperty = g.Sum(x => x.SharePercentageOfWholePropert),
        PriceOfRequest = g.Sum(x => x.SharePrice),

        // Get location info
        City = g.First().Property.Location.City,
        Country = g.First().Property.Location.Country,

        // Get one image URL
        urlImage = g.First().Property.lstPropertiesImage_Document
            .Where(f => f.fileType.ToLower().Contains("image"))
            .Select(f => f.ImageName)
            .FirstOrDefault(),

        // Determine property type using type name
        type = g.First().Property is ModelVilla ? "Villa" :
               g.First().Property is ModelHouse ? "House" :
               g.First().Property is ModelApartment ? "Apartment" : "Unknown"
    })
    .ToListAsync();

            
         return result;

        }

        /////////////////////////////////////
        ///
        public async Task<(bool, string, object)> RequestSaleDetail(int id)
        {
            var request = await _dbContext.RequestForSales
                .Include(r => r.lstShare)
                    .ThenInclude(s => s.InternalOwner)
                .Include(r => r.lstShare)
                    .ThenInclude(s => s.ExternalOwner)
                .Include(r => r.lstShare)
                    .ThenInclude(s => s.Property)
                        .ThenInclude(p => p.Location)
                .Include(r => r.lstShare)
                    .ThenInclude(s => s.Property)
                        .ThenInclude(p => p.lstPropertiesImage_Document)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (request == null)
                return (false, "Request not found", null);

            var property = request.lstShare.FirstOrDefault()?.Property;

            if (property == null)
                return (false, "No property linked", null);

            string propertyType = property.GetType().Name;

            var result = new
            {
                RequestID = request.ID,
                request.RequestDate,
                request.TotalAskingPrice,
                request.SaleType,

                PropertyType = propertyType,
                PropertyID = property.ID,
                property.PropertyCode,
                property.Bedrooms,
                property.Bathrooms,
                property.SpaceInSquareMeter,
                property.YearOfBuilt,
                property.Furnished,
                property.Price,
                property.Accepted,
                property.Rejected,

                VillaDetails = property is ModelVilla villa ? new
                {
                    villa.NumberOfFloor,
                    villa.GardenArea,
                    villa.ISThereSwimmingPool,
                    villa.IsThereGarage
                } : null,

                HouseDetails = property is ModelHouse house ? new
                {
                    house.Backyard,
                    house.ISThereSwimmingPool,
                    house.IsThereGarage,
                    house.NumberOfFloor,
                    house.GardenArea
                } : null,

                ApartmentDetails = property is ModelApartment apt ? new
                {
                    apt.FloorNumber,
                    apt.ApartmentNumber,
                    apt.BuildingName,
                    apt.IsThereParkingSpace
                } : null,

                Location = new
                {
                    property.Location?.Latitude,
                    property.Location?.Longitude,
                    property.Location?.City,
                    property.Location?.Country,
                    property.Location?.Address,
                    property.Location?.ZipCode
                },

                PropertyImages = property.lstPropertiesImage_Document.Select(f => new
                {
                    FileID = f.ID,
                    ImageUrl = "/uploads/" + f.ImageName
                }),

                Shares = request.lstShare.Select(share => new
                {
                    ShareID = share.ID,
                    share.SharePercentageOfWholePropert,
                    share.SharePrice,
                    share.IsExternalOwner,
                    share.IsAvailableForResale,
                    OwnerID =  share.InternalOwnerID,
                    OwnerName =  share.InternalOwner?.UserName
                }).ToList()
            };

            return (true, "Successfully returned", result);
        }





    }
}