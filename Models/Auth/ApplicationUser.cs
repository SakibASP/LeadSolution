using Microsoft.AspNetCore.Identity;

namespace Core.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? Company { get; set; }
        public int? ServiceTypeId { get; set; }
    }
}
