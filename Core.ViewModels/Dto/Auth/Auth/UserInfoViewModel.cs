using Core.ViewModels.Dto.Common;

namespace Core.ViewModels.Dto.Auth.Auth;

public class UserInfoViewModel
{
    public string? UserId { get; set; }
    public string AccessToken { get; set; } = default!;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public IList<DropdownDto>? UserBusinessList { get; set; }
    public IList<string>? UserRoles { get; set; }
}
