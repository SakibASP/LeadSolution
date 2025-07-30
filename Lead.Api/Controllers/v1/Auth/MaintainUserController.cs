using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Request.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead.Api.Controllers.v1.Auth;

/// <summary>
/// Md. Sakibur Rahman
/// 10 July 2025
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = Constants.AdminAuthRoles)]
public class MaintainUserController(
    UserManager<ApplicationUser> userManager,
    IAdminRightsService adminRights) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IAdminRightsService _iAdminRights = adminRights;

    [HttpGet("get-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesDto>();
        foreach (ApplicationUser user in users)
        {
            var thisViewModel = new UserRolesDto
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                NID = user.NID,
                Roles = await GetRoleNames(user)
            };
            userRolesViewModel.Add(thisViewModel);
        }
        if (!isSuperAdmin) userRolesViewModel = [.. userRolesViewModel.Where(x => !x.Roles!.Any(x => x.Equals(Constants.SuperAdmin, StringComparison.CurrentCultureIgnoreCase)))];
        return Ok(ApiResponse<IList<UserRolesDto>>.Success(userRolesViewModel, "Users get successfully"));
    }

    [HttpGet("get-user-roles")]
    public async Task<IActionResult> GetUserRoles([FromQuery] string userId)
    {
        var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
        var roles = await _iAdminRights.GetRoleListAsync(isSuperAdmin);
        var model = new List<ManageUserRoleDto>();
        var user = await _userManager.FindByIdAsync(userId);
        if (roles.Data is not null && user is not null)
        {
            foreach (var role in roles.Data)
            {
                var userRolesViewModel = new ManageUserRoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name!))
                    userRolesViewModel.Selected = true;
                else
                    userRolesViewModel.Selected = false;

                model.Add(userRolesViewModel);
            }
        }
        return Ok(ApiResponse<IList<ManageUserRoleDto>>.Success(model, "User roles get successfully"));
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] UpdateUserRoleRequest request)
    {
        if (string.IsNullOrEmpty(request?.UserId)) return Ok(ApiResponse<string>.Fail("No user found!"));

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null || request.ManageUsers is null) return Ok(ApiResponse<string>.Fail("No user found!"));

        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);

        if (!result.Succeeded) return Ok(ApiResponse<string>.Fail("Something went wrong!"));

        result = await _userManager.AddToRolesAsync(user, request.ManageUsers.Where(x => x.Selected).Select(y => y.RoleName)!);

        if (!result.Succeeded) return Ok(ApiResponse<string>.Fail("Something went wrong!"));

        return Ok(ApiResponse<string>.Success(null, "Role assigned successfully"));
    }

    private async Task<List<string>> GetRoleNames(ApplicationUser user)
    {
        return [.. await _userManager.GetRolesAsync(user)];
    }

}
