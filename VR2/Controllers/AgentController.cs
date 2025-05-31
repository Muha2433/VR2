using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using VR2.DTOqMoels;
using VR2.Models;
using VR2.Services;

namespace VR2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly PropertiesService _propertiesService;
        private readonly AgentService _agentService;
        private readonly ApplicationDbContext _dbContext;
        public AgentController(PropertiesService propertiesService,
            AgentService agentService,
             ApplicationDbContext dbContext) 
        { 
            _dbContext = dbContext;
            _propertiesService = propertiesService;
            _agentService = agentService;
        }

     


        [HttpGet("listCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            var (success, users, message) = await _agentService.GetListOfUname();

            if (!success)
                return BadRequest(new { Message = message });

            return Ok(users);
        }

        [HttpPost("AgentDecision")]
        public async Task<IActionResult> DecideOnProperty([FromBody] AgentDecisionRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                                 .Values
                                 .SelectMany(v => v.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();

                    return BadRequest(new { message = "Validation failed", errors });
                }

                var (success, message) = await _agentService.AcceptOrRejectAsync(request);

                if (!success)
                {
                    return BadRequest(new { message });
                }

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// 


        [HttpGet("AgentDecisionPageForProperty")]
        public async Task<IActionResult> ViewAgentDesignPageForProperty()     
        {
            try
            {
                var result = await _propertiesService.DecsionPropertyList();



                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
