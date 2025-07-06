using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? Company { get; set; }
        public int? ServiceTypeId { get; set; }

        [ForeignKey("ServiceTypeId")]
        public AspNetServiceTypes? ServiceType { get; set; }
    }
}
