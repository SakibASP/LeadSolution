using Models.Auth;
using System.Security.Claims;

namespace Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
