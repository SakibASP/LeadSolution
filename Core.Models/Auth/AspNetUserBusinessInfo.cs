using Core.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;

public class AspNetUserBusinessInfo : BaseModel
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    public int? BusinessId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey(nameof(BusinessId))]
    public virtual AspNetBusinessInfo? AspNetBusinessInfo { get; set; }
}
