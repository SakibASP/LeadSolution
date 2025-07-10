using Application.Interfaces.Auth;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Auth
{
    public class TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, user.Id), // ✅ User ID for User.FindFirstValue
                            new(ClaimTypes.Name, user.UserName ?? string.Empty),
                            new(ClaimTypes.Email, user.Email ?? string.Empty),
                            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

            // Add each role as a separate claim
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Don't validate lifetime here
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.ValidIssuer,
                ValidAudience = _jwtOptions.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
