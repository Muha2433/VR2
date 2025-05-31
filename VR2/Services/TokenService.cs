using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VR2.Models;

namespace VR2.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true, // Ensure the issuer is validated
                ValidateAudience = true, // Validate the audience
                ValidateLifetime = true, // Validate token expiration
                ValidateIssuerSigningKey = true, // Ensure the signing key is valid
                ValidIssuer = configuration["Jwt:Issuer"], // The expected issuer
                ValidAudience = configuration["Jwt:Audience"], // The expected audience
                IssuerSigningKey = GetSymmetricSecurityKey(configuration) // The signing key used for validation
            };
        }

        public string GenerateJwtToken(AppUser user, string role, IEnumerable<Claim> additionalClaims = null)
        {
            // Create a list of claims for the token
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, role)
        };

            // Add additional claims if provided
            if (additionalClaims?.Any() == true)
            {
                claims.AddRange(additionalClaims);
            }

            // Generate the JWT token
            return GenerateJwt(claims);
        }

        private string GenerateJwt(IEnumerable<Claim> claims)
        {
            // Get the security key
            var securityKey = GetSymmetricSecurityKey(_configuration);

            // Create signing credentials
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Get token expiration time from configuration
            var expireInMinutes = Convert.ToInt32(_configuration["Jwt:ExpireInMinutes"] ?? "60");

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // The issuer
                audience: _configuration["Jwt:Audience"], // The audience
                claims: claims, // The claims
                expires: DateTime.UtcNow.AddMinutes(expireInMinutes), // Token expiration time
                signingCredentials: credentials // Signing credentials
            );

            // Write the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration configuration)
        {
            // Retrieve the secret key from configuration
            var key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            // Convert the key into a byte array using UTF8 encoding
            var keyBytes = Encoding.UTF8.GetBytes(key);

            return new SymmetricSecurityKey(keyBytes);
        }
    }

}
