using Core.ViewModels.Dto.Auth;

namespace Core.ViewModels.Request.Auth;

public class UpdateUserRoleRequest
{
    public IList<ManageUserRoleDto>? ManageUsers { get; set; }
    public string? UserId { get; set; }
}
