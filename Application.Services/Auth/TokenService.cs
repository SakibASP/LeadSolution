using Application.Interfaces.Auth;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Auth;

public class TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The application user for whom to generate the token.</param>
    /// <returns>A JWT token string containing user claims and roles.</returns>
    public async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        // Get roles assigned to the user
        var userRoles = await _userManager.GetRolesAsync(user);

        // Create claims for the token
        var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, user.Id), // User ID for User.FindFirstValue
                            new(ClaimTypes.Name, user.UserName ?? string.Empty),
                            new(ClaimTypes.Email, user.Email ?? string.Empty),
                            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
                        };

        // Add each role as a separate claim
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create signing credentials using the secret key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityInMinutes),
            claims: claims,
            signingCredentials: creds
        );

        // Return the serialized token string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    /// <summary>
    /// Generates a secure random refresh token
    /// </summary>
    /// <returns></returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }


    /// <summary>
    /// Extracts ClaimsPrincipal from an expired JWT token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="SecurityTokenException"></exception>
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

        // Ensure the token is a valid JWT and uses the expected algorithm
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
