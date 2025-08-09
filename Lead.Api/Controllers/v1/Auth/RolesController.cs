using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data;

namespace Lead.Api.Controllers.v1.Auth;

/// <summary>
/// Md. Sakibur Rahman
/// 10 July 2025
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = Constants.AdminAuthRoles)]
public class RolesController(
    RoleManager<IdentityRole> roleManager,
    IAdminRightsService adminRights,
    IHttpContextAccessor httpContext) : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IAdminRightsService _iAdminRights = adminRights;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";


    [HttpGet("all-roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
            var roles = await _iAdminRights.GetRoleListAsync(isSuperAdmin);
            return Ok(ApiResponse<IList<IdentityRole>>.Success(roles.Data, "Roles get successfully!")); // returns JSON list of roles
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting roles");
            return Ok(ApiResponse<IList<IdentityRole>>.Fail("No role found!"));
        }
    }

    [HttpGet("get-role-by-Id")]
    public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            return Ok(ApiResponse<IdentityRole>.Success(role, "Role get successfully!"));
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting roles {roleId}", roleId);
            return Ok(ApiResponse<IdentityRole>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Ok(ApiResponse<string>.Fail("Role create failed!"));

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return Ok(ApiResponse<string>.Fail("Role already exists!"));

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded ? Ok(ApiResponse<string>.Success(null, "Role created successfully!")) : Ok(ApiResponse<string>.Fail("Role already exists!"));
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error creating role {roleName}", roleName);
            return Ok(ApiResponse<IList<IdentityRole>>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("update-role")]
    public async Task<IActionResult> UpdateRole([FromBody] RoleDto role)
    {
        try
        {
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var _role = await _roleManager.FindByIdAsync(role.Id);
            if (_role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var setNameResult = await _roleManager.SetRoleNameAsync(_role, role.Name);
            if (!setNameResult.Succeeded)
                return Ok(ApiResponse<string>.Fail("Failed to set role name."));

            var updateResult = await _roleManager.UpdateAsync(_role);
            if (!updateResult.Succeeded)
                return Ok(ApiResponse<string>.Fail("Failed to update role."));

            return Ok(ApiResponse<string>.Success(null, "Role updated successfully!"));
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updaing role {role}", role);
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("delete-role")]
    public async Task<IActionResult> DeleteRole([FromBody] string roleId)
    {
        try
        {
            if (string.IsNullOrEmpty(roleId))
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            await _roleManager.DeleteAsync(role);
            return Ok(ApiResponse<string>.Success(null, "Role removed successfully!"));
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error deleting role {role}", roleId);
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }
}
