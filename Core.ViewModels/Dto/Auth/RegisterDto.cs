using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Dto.Auth;

public class RegisterDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    [Display(Name = "Phone Number")]
    public required string PhoneNumber { get; set; }
    public required string Company { get; set; }
    [Display(Name = "Service Type")]
    public required int ServiceTypeId { get; set; }
}
