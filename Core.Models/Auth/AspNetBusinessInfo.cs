using Core.Models.Common;
using Core.Models.Lead;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;

public class AspNetBusinessInfo : BaseModel
{
    [Required]
    public int ServiceId { get; set; }

    [Required, StringLength(128)]
    public string BusinessName { get; set; }

    [Required, StringLength(56)]
    public string Phone { get; set; }

    [Required, StringLength(56)]
    public string Email { get; set; }

    [Required, StringLength(56)]
    public string WhatsApp { get; set; }

    [Required, StringLength(56)]
    public string Website { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Required]
    public bool IsActive { get; set; } = false;


    [ForeignKey(nameof(ServiceId))]
    public virtual AspNetServiceTypes? AspNetServiceTypes { get; set; }
    // Navigation property
    public virtual ICollection<AspNetUserBusinessInfo>? UserBusinessInfos { get; set; }
    public virtual ICollection<FormValues>? FormValues { get; set; }
}
