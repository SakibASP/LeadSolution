namespace Core.ViewModels.Dto.Auth.Roles;

public class UserRolesDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? NID { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
