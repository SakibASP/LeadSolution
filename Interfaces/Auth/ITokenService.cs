using Core.Models.Auth;
using System.Security.Claims;

namespace Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
