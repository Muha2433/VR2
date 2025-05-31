using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Xml;
using VR2.DTOqMoels;
using VR2.Models;
using VR2.MyTools;

namespace VR2.Services
{
    public class PropertiesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly FileTools _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // private readonly UserManager<AppUser> _userManager;

        public PropertiesService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            //  UserManager<AppUser> userManager
            FileTools fileService)
        {
            _dbContext = dbContext;
            _httpContextAccessor=httpContextAccessor;
            //_userManager = userManager;
            _fileService = fileService;
        }

        public async Task<(bool success, string message, ModelApartment model)> CreateApartment(DtoApartment dto)
        {
            // ========== 1. INPUT VALIDATION ==========
            if (dto == null || dto.Location == null)
                return (false, "Property or location data is missing.", null);

            // ========== 2. PROPERTY CODE CHECK ==========
            var existing = await _dbContext.Properties
                .FirstOrDefaultAsync(p => p.PropertyCode == dto.PropertyCode);

            if (existing != null)
            {
                return existing switch
                {
                    { Accepted: false, Rejected: false } =>
                        (false, "A property with this code has already been uploaded. Please wait for verification.", null),
                    { Accepted: true, Rejected: false } =>
                        (false, "A property with this code has already been approved.", null),
                    _ => (false, "A property with this code exists but was rejected.", null)
                };
            }

            // ========== 3. FILE VALIDATION ==========
            var fileValidation = ValidatePropertyFiles(dto.lstPropertiesImage_Document);
            if (!fileValidation.success)
                return (fileValidation.success, fileValidation.message, null);

            // ========== 4. LOCATION PROCESSING ==========
            var locationResult = await ProcessLocation(dto.Location);
            if (!locationResult.success)
                return (locationResult.success, locationResult.message, null);

            // ========== 5. FILE SAVING ==========
            var savedFiles = await _fileService.SaveUserFiles(dto.lstPropertiesImage_Document);
            if (!savedFiles.Any())
                return (false, "Failed to save property files.", null);

            // ========== 6. PROPERTY CREATION ==========
            var newProperty = new ModelApartment
            {
                PropertyCode = dto.PropertyCode,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                SpaceInSquareMeter = dto.SpaceInSquareMeter,
                YearOfBuilt = dto.YearOfBuilt,
                Furnished = dto.Furnished,
                Price = dto.Price,
                LocationID = locationResult.locationId,
                Location = locationResult.location,
                Accepted = false,
                Rejected = false,
                FloorNumber = dto.FloorNumber,
                ApartmentNumber = dto.ApartmentNumber,
                BuildingName = dto.BuildingName,
                IsThereParkingSpace = dto.IsThereParkingSpace,
            };


            try
            {
                _dbContext.Apartments.Add(newProperty);
                await _dbContext.SaveChangesAsync();
                List<ModelFile> lstFile = new List<ModelFile>();
                foreach (var (filePath, fileType) in savedFiles)
                {
                    lstFile.Add(new ModelFile
                    {
                        fileType = fileType,
                        ImageName = filePath,
                        PropertyID = newProperty.ID,
                        Property = newProperty
                    });
                }
                _dbContext.AddRange(lstFile);
                await _dbContext.SaveChangesAsync();
                newProperty.lstPropertiesImage_Document = lstFile;
                _dbContext.Entry(newProperty).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
            // ========== 7. SHARES PROCESSING ==========
            //var sharesResult = await ProcessShares(newProperty.ID, dto.lstShare);
            //if (!sharesResult.success)
            //{
            //    // Rollback property creation if shares fail
            //    _dbContext.Properties.Remove(newProperty);
            //    await _dbContext.SaveChangesAsync();
            //    return (sharesResult.success, sharesResult.message,null);
            //}

            // ========== 8. FILE ASSOCIATION ==========
            // (Implement this if you have a file tracking system)
            // await AssociateFilesWithProperty(newProperty.ID, savedFiles);



            return (true, "Property added successfully.", newProperty);
        }




        public async Task<(bool success, string message, ModelVilla model)> CreateVilla(DtoVilla dto)
        {
            // ========== 1. INPUT VALIDATION ==========
            if (dto == null || dto.Location == null)
                return (false, "Property or location data is missing.", null);

            // ========== 2. PROPERTY CODE CHECK ==========
            var existing = await _dbContext.Properties
                .FirstOrDefaultAsync(p => p.PropertyCode == dto.PropertyCode);

            if (existing != null)
            {
                return existing switch
                {
                    { Accepted: false, Rejected: false } =>
                        (false, "A property with this code has already been uploaded. Please wait for verification.", null),
                    { Accepted: true, Rejected: false } =>
                        (false, "A property with this code has already been approved.", null),
                    _ => (false, "A property with this code exists but was rejected.", null)
                };
            }

            // ========== 3. FILE VALIDATION ==========
            var fileValidation = ValidatePropertyFiles(dto.lstPropertiesImage_Document);
            if (!fileValidation.success)
                return (fileValidation.success, fileValidation.message, null);

            // ========== 4. LOCATION PROCESSING ==========
            var locationResult = await ProcessLocation(dto.Location);
            if (!locationResult.success)
                return (locationResult.success, locationResult.message, null);

            // ========== 5. FILE SAVING ==========
            var savedFiles = await _fileService.SaveUserFiles(dto.lstPropertiesImage_Document);
            if (!savedFiles.Any())
                return (false, "Failed to save property files.", null);

            // ========== 6. PROPERTY CREATION ==========
            var newProperty = new ModelVilla
            {
                PropertyCode = dto.PropertyCode,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                SpaceInSquareMeter = dto.SpaceInSquareMeter,
                YearOfBuilt = dto.YearOfBuilt,
                Furnished = dto.Furnished,
                Price = dto.Price,
                LocationID = locationResult.locationId,
                Location = locationResult.location,
                Accepted = false,
                Rejected = false,
                NumberOfFloor = dto.NumberOfFloor,
                GardenArea = dto.GardenArea,
                ISThereSwimmingPool = dto.ISThereSwimmingPool,
                IsThereGarage = dto.IsThereGarage
            };

            try
            {
                _dbContext.Villas.Add(newProperty);
                await _dbContext.SaveChangesAsync();
                List<ModelFile> lstFile = new List<ModelFile>();
                foreach (var (filePath, fileType) in savedFiles)
                {
                    lstFile.Add(new ModelFile
                    {
                        fileType = fileType,
                        ImageName = filePath,
                        PropertyID = newProperty.ID,
                        Property = newProperty
                    });
                }
                _dbContext.AddRange(lstFile);
                await _dbContext.SaveChangesAsync();
                newProperty.lstPropertiesImage_Document = lstFile;
                _dbContext.Entry(newProperty).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
            // ========== 7. SHARES PROCESSING ==========
            //var sharesResult = await ProcessShares(newProperty.ID, dto.lstShare);
            //if (!sharesResult.success)
            //{
            //    // Rollback property creation if shares fail
            //    _dbContext.Properties.Remove(newProperty);
            //    await _dbContext.SaveChangesAsync();
            //    return (sharesResult.success, sharesResult.message,null);
            //}

            // ========== 8. FILE ASSOCIATION ==========
            // (Implement this if you have a file tracking system)
            // await AssociateFilesWithProperty(newProperty.ID, savedFiles);



            return (true, "Property added successfully.", newProperty);
        }




        public async Task<(bool success, string message, ModelHouse model)> CreateHouse(DtoHouse dto)
        {
            // ========== 1. INPUT VALIDATION ==========
            if (dto == null || dto.Location == null)
                return (false, "Property or location data is missing.", null);

            // ========== 2. PROPERTY CODE CHECK ==========
            var existing = await _dbContext.Properties
                .FirstOrDefaultAsync(p => p.PropertyCode == dto.PropertyCode);

            if (existing != null)
            {
                return existing switch
                {
                    { Accepted: false, Rejected: false } =>
                        (false, "A property with this code has already been uploaded. Please wait for verification.", null),
                    { Accepted: true, Rejected: false } =>
                        (false, "A property with this code has already been approved.", null),
                    _ => (false, "A property with this code exists but was rejected.", null)
                };
            }

            // ========== 3. FILE VALIDATION ==========
            var fileValidation = ValidatePropertyFiles(dto.lstPropertiesImage_Document);
            if (!fileValidation.success)
                return (fileValidation.success, fileValidation.message, null);

            // ========== 4. LOCATION PROCESSING ==========
            var locationResult = await ProcessLocation(dto.Location);
            if (!locationResult.success)
                return (locationResult.success, locationResult.message, null);

            // ========== 5. FILE SAVING ==========
            var savedFiles = await _fileService.SaveUserFiles(dto.lstPropertiesImage_Document);
            if (!savedFiles.Any())
                return (false, "Failed to save property files.", null);

            // ========== 6. PROPERTY CREATION ==========
            var newProperty = new ModelHouse
            {
                PropertyCode = dto.PropertyCode,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                SpaceInSquareMeter = dto.SpaceInSquareMeter,
                YearOfBuilt = dto.YearOfBuilt,
                Furnished = dto.Furnished,
                Price = dto.Price,
                LocationID = locationResult.locationId,
                Location = locationResult.location,
                Accepted = false,
                Rejected = false,
                Backyard = dto.Backyard,
                IsThereGarage = dto.IsThereGarage,
                ISThereSwimmingPool = dto.ISThereSwimmingPool,
                GardenArea = dto.GardenArea,
                NumberOfFloor = dto.NumberOfFloor
            };


            try
            {
                _dbContext.Houses.Add(newProperty);
                await _dbContext.SaveChangesAsync();
                List<ModelFile> lstFile = new List<ModelFile>();
                foreach (var (filePath, fileType) in savedFiles)
                {
                    lstFile.Add(new ModelFile
                    {
                        fileType = fileType,
                        ImageName = filePath,
                        PropertyID = newProperty.ID,
                        Property = newProperty
                    });
                }
                _dbContext.AddRange(lstFile);
                await _dbContext.SaveChangesAsync();
                newProperty.lstPropertiesImage_Document = lstFile;
                _dbContext.Entry(newProperty).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
            // ========== 7. SHARES PROCESSING ==========
            //var sharesResult = await ProcessShares(newProperty.ID, dto.lstShare);
            //if (!sharesResult.success)
            //{
            //    // Rollback property creation if shares fail
            //    _dbContext.Properties.Remove(newProperty);
            //    await _dbContext.SaveChangesAsync();
            //    return (sharesResult.success, sharesResult.message,null);
            //}

            // ========== 8. FILE ASSOCIATION ==========
            // (Implement this if you have a file tracking system)
            // await AssociateFilesWithProperty(newProperty.ID, savedFiles);



            return (true, "Property added successfully.", newProperty);
        }



        // ========== HELPER METHODS ==========

        private (bool success, string message) ValidatePropertyFiles(ICollection<FileTuple> files)
        {
            if (files == null || !files.Any())
                return (false, "Please upload at least one property image and one document.");

            var propertyImages = files
                .Where(f => f.FileType?.Equals("property image", StringComparison.OrdinalIgnoreCase) == true)
                .SelectMany(f => f.lstRealFile)
                .ToList();

            var propertyDocuments = files
                .Where(f => f.FileType?.Equals("property document", StringComparison.OrdinalIgnoreCase) == true)
                .SelectMany(f => f.lstRealFile)
                .ToList();

            if (!propertyImages.Any())
                return (false, "Please upload at least one property image.");

            if (!propertyDocuments.Any())
                return (false, "Please upload at least one property document.");

            // Validate image file types
            foreach (var image in propertyImages)
            {
                if (!_fileService.IsImage(image))
                    return (false, $"File {image.FileName} is not a valid image.");
            }

            return (true, null);
        }

        private async Task<(bool success, string message, int locationId, ModelLocation location)> ProcessLocation(DtoLocation locationDto)
        {
            var existingLocation = await _dbContext.Locations.FirstOrDefaultAsync(l =>
                l.Latitude == locationDto.Latitude &&
                l.Longitude == locationDto.Longitude &&
                l.Country == locationDto.Country &&
                l.City == locationDto.City &&
                l.Address == locationDto.Address &&
                l.ZipCode == locationDto.ZipCode);

            if (existingLocation != null)
                return (true, null, existingLocation.Id, existingLocation);

            var newLocation = new ModelLocation
            {
                Latitude = locationDto.Latitude,
                Longitude = locationDto.Longitude,
                Country = locationDto.Country,
                City = locationDto.City,
                Address = locationDto.Address,
                ZipCode = locationDto.ZipCode
            };

            _dbContext.Locations.Add(newLocation);
            await _dbContext.SaveChangesAsync();

            return (true, null, newLocation.Id, newLocation);
        }

        //private async Task<(bool success, string message)> ProcessShares(int propertyId, ICollection<DtoShare> shares)
        //{
        //    if (shares == null || !shares.Any())
        //        return (false, "Please add shares for the property.");

        //    var shareList = new List<ModelShare>();
        //    foreach (var share in shares)
        //    {
        //        var newShare = new ModelShare
        //        {
        //            SharePrice = share.SharePrice,
        //            PurchasePrice = share.PurchasePrice,
        //            SharePercentageOfWholePropert = share.SharePercentageOfWholePropert,
        //            IsAvailableForResale = false,
        //            PropertyID = propertyId,
        //            IsExternalOwner = (bool)share.IsExternalOwner
        //        };

        //        if ((bool)share.IsExternalOwner)
        //        {
        //            newShare.ExternalOwnerID = share.External_Seller_ID;
        //        }
        //        else
        //        {
        //            newShare.InternalOwnerID = share.Internal_Seller_ID;
        //        }

        //        shareList.Add(newShare);
        //    }

        //    _dbContext.Shares.AddRange(shareList);
        //    await _dbContext.SaveChangesAsync();

        //    return (true, null);
        //}

        /////////////////////////////////////////////////////
        ///Accept Reject Pending Properties
        ///
        public async Task<List<PropertyDecisionDto>> DecsionPropertyList()
        {
            try
            {
                var lstProperties = await _dbContext.Properties
                    .AsNoTracking()  // Add this to prevent tracking
                    .Include(e => e.lstPropertiesImage_Document)
                    .ToListAsync();

                var result = new List<PropertyDecisionDto>();
                var request = _httpContextAccessor.HttpContext.Request;

                foreach (var p in lstProperties)
                {
                    string type = p switch
                    {
                        ModelVilla _ => "Villa",
                        ModelApartment _ => "Apartment",
                        ModelHouse _ => "House",
                        _ => "Property"
                    };

                    var relativeImages = p.lstPropertiesImage_Document?
                        .Where(i => i.ImageName != null && i.fileType == "property image")
                        .Select(i => i.ImageName)
                        .ToList() ?? new List<string>();


                    var fullImageUrls = relativeImages
                .Select(img => _fileService.GetFullFileUrl(img, request))
                .ToList();

                    result.Add(new PropertyDecisionDto
                    {
                        ImageUrls = fullImageUrls,
                        Type = type,
                        Id = p.ID.ToString(),
                        DisplayImage = fullImageUrls.FirstOrDefault() ?? _fileService.GetFullFileUrl("/question.png", request)
                    });
                }

               return result;
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error in DecidePropertyAdding");
                throw; // Re-throw to let the controller handle it
            }
        }


        public async Task<(bool,PropertyDetailsResponseDto,string)> GetDetailProperty(int propertyID)
        {
            if (propertyID == null) return (false,null, "proertyID not approperiate");

            var property = await _dbContext.Properties.Include(p => p.Location)
                .Include(listdocumentimage => listdocumentimage.lstPropertiesImage_Document)
                .FirstOrDefaultAsync(p=>p.ID==propertyID);
            //
            if (property == null) return (false,null,"proerty not found");

            var request = _httpContextAccessor.HttpContext.Request;
            var response = new PropertyDetailsResponseDto();

            // Map base property info
            response.BaseProperty = new DtoProperty
            {
                ID = property.ID,
                PropertyCode = property.PropertyCode,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                SpaceInSquareMeter = property.SpaceInSquareMeter,
                YearOfBuilt = property.YearOfBuilt,
                Furnished = property.Furnished,
                Price = property.Price,
                Location = new DtoLocation
                {
                    Address = property.Location?.Address,
                    City = property.Location?.City,
                    Country = property.Location?.Country,
                    ZipCode = property.Location?.ZipCode,
                    Latitude = property.Location?.Latitude ?? 0,
                    Longitude = property.Location?.Longitude ?? 0
                },
            };

            // Process files
            foreach (var file in property.lstPropertiesImage_Document) 
            {
                var url = _fileService.GetFullFileUrl(file.ImageName, request);

                if (file.fileType?.Equals("property image", StringComparison.OrdinalIgnoreCase) == true)
                {
                    response.ImageUrls.Add(url);
                }
                else
                {
                    response.Documents.Add(new DocumentInfoDto
                    {
                        FileName = Path.GetFileName(file.ImageName),
                        DownloadUrl = url,
                        FileType = file.fileType
                    });
                }

            }


         

            // Add type-specific details
            switch (property)
            {
                case ModelApartment apt:
                    response.Type = "Apartment";
                    response.ApartmentDetails = new DtoApartment
                    {
                        FloorNumber = apt.FloorNumber,
                        ApartmentNumber = apt.ApartmentNumber,
                        BuildingName = apt.BuildingName,
                        IsThereParkingSpace = apt.IsThereParkingSpace
                    };
                    break;

                case ModelVilla villa:
                    response.Type = "Villa";
                    response.VillaDetails = new DtoVilla
                    {
                        NumberOfFloor = villa.NumberOfFloor,
                        GardenArea = villa.GardenArea,
                        ISThereSwimmingPool = villa.ISThereSwimmingPool,
                        IsThereGarage = villa.IsThereGarage
                    };
                    break;

                case ModelHouse house:
                    response.Type = "House";
                    response.HouseDetails = new DtoHouse
                    {
                        Backyard = house.Backyard,
                        IsThereGarage = house.IsThereGarage,
                        ISThereSwimmingPool = house.ISThereSwimmingPool,
                        GardenArea = house.GardenArea,
                        NumberOfFloor = house.NumberOfFloor
                    };
                    break;
            }

            return (true, response,"Successfully");

        }



        public async Task<(bool,int)> IsShareExist(int shareID)
        {
            if (shareID==null || shareID == 0 ) return (false,-1);
            var share=await _dbContext.Shares.FindAsync(shareID);
            if (share == null) return (false, -1);
            return (true,share.PropertyID);
        }















        ////////public async Task<List<RequestSaleDto>> PropertyListForRecommendationSystem()
        ////////{
        ////////    try
        ////////    {

        ////////    }
        ////////    catch (Exception ex)
        ////////    {
        ////////        // _logger.LogError(ex, "Error in DecidePropertyAdding");
        ////////        throw; // Re-throw to let the controller handle it
        ////////    }
        ////////}

    }
}
