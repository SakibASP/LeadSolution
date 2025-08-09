using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Controllers.v1.Menu;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = Constants.AdminAuthRoles)]
public class AdminRightsController(IAdminRightsService adminRights, IHttpContextAccessor httpContext) : Controller
{
    private readonly IAdminRightsService _iAdminRights = adminRights;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    [HttpGet("get-all-menu")]
    public async Task<IActionResult> GetAllMenu()
    {
        try
        {
            var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
            var menu = await _iAdminRights.GetMenuMasterAsync(isSuperAdmin);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting all menu");
            return Ok(ApiResponse<IList<MenuItem>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-role-wise-menu")]
    public async Task<IActionResult> GetRoleWiseMenu([FromQuery] string roleId)
    {
        try
        {
            var menu = await _iAdminRights.GetRoleWiseSelectedPagesAsync(roleId);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting role-wise menu for RoleId: {RoleId}", roleId);
            return Ok(ApiResponse<IList<MenuItemViewModel>>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("create-role-wise-menu")]
    public async Task<IActionResult> CreateRoleWiseMenu([FromBody] MenuItemViewModel menuItem)
    {
        try
        {
            var menu = await _iAdminRights.CreateAsync(menuItem);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error creating role-wise menu: {@MenuItem}", menuItem);
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("update-role-wise-menu")]
    public async Task<IActionResult> UpdateRoleWiseMenu([FromBody] UpdateAdminRightsRequest updateRequest)
    {
        try
        {
            var menu = await _iAdminRights.UpdateRecordsAsync(updateRequest);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating role-wise menu: {@UpdateRequest}", updateRequest);
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

}
