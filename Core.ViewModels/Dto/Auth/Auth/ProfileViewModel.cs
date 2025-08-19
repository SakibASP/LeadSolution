using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Dto.Auth.Auth;

public class ProfileViewModel
{

    [Display(Name = "User Name")]
    public string? UserName { get; set; }

    [Display(Name = "NID")]
    public string? NID { get; set; }

    [Required, EmailAddress, Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone, Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Password), Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password), Display(Name = "New Password")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password), Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
}
