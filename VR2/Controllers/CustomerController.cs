using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VR2.DataBaseServices;
using VR2.DTOqMoels;
using VR2.HubRealTime_Playing;
using VR2.Models;
using VR2.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VR2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerServices _Customerservices;
        private readonly PropertiesService _Propertiesservices;
        private readonly IGroupSaleTracker _GroupSaleTracker;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly crudRequestSale _crudRequestSale;
        public CustomerController(CustomerServices customerservices,
            PropertiesService propertiesService,  IGroupSaleTracker groupSaleTracker,
            IHubContext<NotificationHub> hubContext, crudRequestSale crudRequestSale)
        {
            _Customerservices = customerservices;
            _Propertiesservices = propertiesService;
            _GroupSaleTracker = groupSaleTracker;
            _crudRequestSale = crudRequestSale;
            _hubContext= hubContext;
        }

        [HttpGet("GetOwnerOfProperty/{IdOfShare}")]
        public async Task<IActionResult> GetOwnerOfProperty(int IdOfShare)
       {
            var result = await _Customerservices.GetOwnerOfProperty(IdOfShare); // Call the method here
            if (!result.Item1)
            {
                return BadRequest(new { message = result.Item2 }); // Return error message if failure
            }

            return Ok(new { message = result.Item2, owners = result.Item3 }); // Return success message with the list of owners
        }

        [HttpGet("ProfileCustomer")]
        public async Task<IActionResult> GetProfileCustomer()
        {

            // Retrieve the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing" });
            }

            // Parse the token to extract the ID
            string userId = GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }
            //
            //await _hubContext.Clients.All.SendAsync("ReceiveGroupSaleRequest", new
            //{
            //    message = "Testing broadcast"
            //});
            // Now you have the user ID, you can pass it to your service method
            var (success, message, shares) = await _Customerservices.GetListOfShare(userId);

            if (!success)
            {
                return BadRequest(new { success, message });
            }

            if (shares == null || !shares.Any())
            {
                shares = new List<ModelShare>();
            }
            
            var (s, m, customer)=await _Customerservices.GetCustomerByID(userId);

            customerProfileDto profil = new customerProfileDto
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                UserName = customer.UserName,
                ImageUrl = "https://kendahar.bsite.net/Uploaded/Document/PropertyDocument/e55dd5a9-6a28-49d9-8558-1c2f1c2a2cde_20250503085305.png"
            };
            var lstshare = new List<ShareDtoMobile>();
            // Map shares to DTO
            profil.lstShareDtoMobile = shares.Select(sh => new ShareDtoMobile
            {
                Id = sh.ID,
                IsAvailableForResale = sh.IsAvailableForResale,
                Percentage = sh.SharePercentageOfWholePropert,
                sharePrice = sh.SharePrice
            }).ToList();

            return Ok(new { success = true, profil });
        }



        [HttpGet("CustomerShare")]
        public async Task<IActionResult> GetShareOfCustomer()     
        {
            // Retrieve the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing" });
            }

            // Parse the token to extract the ID
            string userId = GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }
            //
         
            // // Now you have the user ID, you can pass it to your service method
             var (success, message, shares) = await _Customerservices.GetListOfShare(userId);

            if (!success)
            {
                return BadRequest(new { success, message });
            }

            if (shares == null || !shares.Any())
            {
                return Ok(new { success, message, shares = new List<ModelShare>() });
            }

            return Ok(new { success, message, shares });
        }

        private string? GetUserIdFromToken(string token)
        {
            try
            {
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Log all claims for debugging
                foreach (var claim in jwtToken?.Claims)
                {
                    
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                // Extract the 'NameIdentifier' claim (which contains the user ID)
                var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }

                return null; // Return null if the claim is not found
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error decoding token: {ex.Message}");
                return null;
            }
        }

        [HttpPost("RequestIndividualSale")]
        public async Task<IActionResult> RequestIndividualSale([FromBody] int ShareId)
        {
            // Retrieve the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing" });
            }

            // Parse the token to extract the ID
            string initiatorAskerID = GetUserIdFromToken(token);

            if (string.IsNullOrEmpty(initiatorAskerID))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var askerName = await _Customerservices.GetUserName(initiatorAskerID);
            if (string.IsNullOrEmpty(askerName))
            {
                return Unauthorized(new { success = false, message = "Invalid user" });
            }

            var shareResult = await _Propertiesservices.IsShareExist(ShareId);
            var share = shareResult.Item1;
            var propertyID = shareResult.Item2;

            if (share == null)
            {
                return NotFound(new { success = false, message = "Share not found" });
            }

            if (propertyID == null)
            {
                return NotFound(new { success = false, message = "Property not found" });
            }

            var dto = new DtoIndividualRequestSale
            {
                AskerId = initiatorAskerID,
                PropertyID = propertyID,
                ShareId = ShareId
            };

            await _crudRequestSale.AddIndividualRequestForSale(dto);

            return Ok(new { success = true, message = "Request submitted successfully" });
        }



        [HttpPost("RequestGroupSale")]

        public async Task<IActionResult> RequestGroupSale([FromBody] AskingRequestDto request)
        {
            // Retrieve the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing" });
            }

            // Parse the token to extract the ID
            string initiatorAskerID = GetUserIdFromToken(token);

            if (initiatorAskerID == null)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var askerName = await _Customerservices.GetUserName(initiatorAskerID); //GetUserName
            if (askerName == null) return Unauthorized(new { success = false, message = "Invalid token" });

            var share = await _Propertiesservices.IsShareExist(request.ShareId);
            if (share.Item1 == null) return NotFound("Share not found.");

            var propertyID = share.Item2;
            if (propertyID == null) { return NotFound("Property Not Found"); }

            var shareTitle = string.Format("Share number {0}", request.ShareId);

            var requestId = Guid.NewGuid().ToString();

            var groupSaleState = new DtoGroupRequestSale
            {    
                ShareId = request.ShareId,
                PropertyID=propertyID,
                AskerId = initiatorAskerID,
                OwnerIds = request.SelectedOwners.Select(o => o.ID).ToList(),
                Approvals = new Dictionary<string, bool?>()
            };

            foreach (var owner in request.SelectedOwners)
            {
                groupSaleState.Approvals[owner.ID] = null;

                //await _hubContext.Clients.User(owner.ID).SendAsync("ReceiveGroupSaleRequest", new
                //{
                //    message = $"{askerName} wants to collaborate with you on selling share {shareTitle}"
                //});

                await _hubContext.Clients.User(owner.ID).SendAsync("ReceiveGroupSaleRequest", new
                {
                    requestId,
                    message = $"{askerName} wants to collaborate with you on selling share {shareTitle}",
                    askerName = askerName,
                    shareId = request.ShareId
                });
            }
      //      groupSaleState.OwnerIds.Add(groupSaleState.AskerId);
       //     groupSaleState.Approvals[groupSaleState.AskerId] = true;

            _GroupSaleTracker.AddRequest(requestId, groupSaleState);

            return Ok(new { requestId });
        }


        [HttpPost("responding")]
        public async Task<IActionResult> RespondToRequest([FromBody] GroupSaleResponseDto response)
        {
            // Retrieve the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing" });
            }

            // Parse the token to extract the ID
            string userId = GetUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var askerName = await _Customerservices.GetUserName(userId); //GetUserName

            // response: { string RequestId, bool Accepted }
            _GroupSaleTracker.RegisterResponse(response.RequestId, userId, response.Accepted);
            return Ok(new { message = response.Accepted ? "Response recorded: Accepted" : "Response recorded: Rejected" });
        }

    }   

}

