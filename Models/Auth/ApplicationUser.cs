using Microsoft.AspNetCore.Identity;

namespace Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        // Properties for refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
