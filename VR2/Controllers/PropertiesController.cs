using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VR2.DTOqMoels;
using VR2.MyTools;
using VR2.Services;

namespace VR2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class PropertiesController : ControllerBase
    {
        private readonly PropertiesService _propertiesService;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(PropertiesService propertiesService, ILogger<PropertiesController> logger)
        {
            _propertiesService = propertiesService;
            _logger = logger;
        }

        

        //[DisableRequestSizeLimit]
        //[RequestFormLimits(MultipartBodyLengthLimit = 20_000_000)]
        [HttpPost("VillaCreation")]
        // [Authorize] // Add this if you want the endpoint to be protected
        public async Task<IActionResult> CreateVilla([FromForm] DtoVilla dtoVilla)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for villa creation");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request data",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                var result = await _propertiesService.CreateVilla(dtoVilla);

                if (!result.success)
                {
                    _logger.LogWarning("Villa creation failed: {Message}", result.message);
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                _logger.LogInformation("Villa created successfully with ID: {VillaId}", result.model.ID);
                return Ok(new
                {
                    success = true,
                    message = result.message,
                    villa = result.model
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating villa");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while creating the villa"
                });
            }
        }



        [HttpPost("HouseCreation")]
        // [Authorize] // Add this if you want the endpoint to be protected
        public async Task<IActionResult> CreateHouse([FromForm] DtoHouse dtoHouse)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for villa creation");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request data",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                var result = await _propertiesService.CreateHouse(dtoHouse);

                if (!result.success)
                {
                    _logger.LogWarning("Villa creation failed: {Message}", result.message);
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                _logger.LogInformation("Villa created successfully with ID: {VillaId}", result.model.ID);
                return Ok(new
                {
                    success = true,
                    message = result.message,
                    villa = result.model
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating villa");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while creating the villa"
                });
            }
        }


        [HttpPost("ApartmentCreation")]
        // [Authorize] // Add this if you want the endpoint to be protected
        public async Task<IActionResult> CreateApartmen([FromForm] DtoApartment dtoApartment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for villa creation");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request data",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                var result = await _propertiesService.CreateApartment(dtoApartment);

                if (!result.success)
                {
                    _logger.LogWarning("Villa creation failed: {Message}", result.message);
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                _logger.LogInformation("Villa created successfully with ID: {VillaId}", result.model.ID);
                return Ok(new
                {
                    success = true,
                    message = result.message,
                    villa = result.model
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating villa");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while creating the villa"
                });
            }
        }

        //
        [HttpGet("PropertyDetails/{id}")]
        public async Task<IActionResult> GetPropertyDetails(int id)
        {
            try
            {
                var result = await _propertiesService.GetDetailProperty(id);

                if (!result.Item1)
                    return NotFound(new { message = result.Item3 });

                return Ok(result.Item2); // Only return the DTO here
            }
            catch (Exception ex)
            {
                return StatusCode (500, new { message = $"Internal server error: {ex.Message}" });
            }
        }



    }
}
