using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Dto.Auth.Auth;

public class RegisterDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    [Display(Name = "Phone Number")]
    public required string PhoneNumber { get; set; }

    [Display(Name = "Smart Card/National Id")]
    public required string NID { get; set; }
}
