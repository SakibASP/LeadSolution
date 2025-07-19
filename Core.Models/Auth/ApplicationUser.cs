using Microsoft.AspNetCore.Identity;

namespace Core.Models.Auth;

public class ApplicationUser : IdentityUser
{
    public string? NID { get; set; }
}
