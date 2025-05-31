using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VR2.DTOqMoels;
using VR2.Services;

namespace VR2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Validate incoming model (email and password)
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Call the LoginUserAsync method to perform login
            var (success, message, token) = await _accountService.LoginUserAsync(loginRequest.Email, loginRequest.Password);

            if (!success)
            {
                return Unauthorized(new { success, message });
            }

            return Ok(new { success, message, token });
        }

        [HttpPost("RegisterC")]
        public async Task<IActionResult> RegisterUserWithImages([FromForm] DtoUser dtoUser)
        {
            var (success, message, token) = await _accountService.RegisterUserWithImages(dtoUser);

            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message, token });
        }
    }

}
