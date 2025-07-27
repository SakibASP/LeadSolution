using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Controllers.v1.Menu;

[ApiController]
[Route("api/v1/[controller]")]
public class AdminRightsController(IAdminRightsService adminRights) : Controller
{
    private readonly IAdminRightsService _iAdminRights = adminRights;

    [Authorize(Roles = Constants.AdminAuthRoles)]
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
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IList<MenuItem>>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
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
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IList<MenuItemViewModel>>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
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
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, menuItem, User.Identity?.Name));
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
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
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, updateRequest, User.Identity?.Name));
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }
}
