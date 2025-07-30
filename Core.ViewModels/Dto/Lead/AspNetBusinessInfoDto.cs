using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Dto.Lead;

public class AspNetBusinessInfoDto
{
    public int? BusinessId { get; set; } // used in edit only

    [Display(Name = "Service")]
    public int ServiceId { get; set; }

    [Display(Name = "Business Name")]
    public string BusinessName { get; set; } = string.Empty;
                 
    public string Phone { get; set; } = string.Empty;
                 
    public string Email { get; set; } = string.Empty;
                 
    public string WhatsApp { get; set; } = string.Empty;
                 
    public string Website { get; set; } = string.Empty;

    public string? Address { get; set; }

    public bool IsActive { get; set; } = false;

    [Display(Name = "Users")]
    public string? UserId { get; set; }

    public string? UserName { get; set; }
}
