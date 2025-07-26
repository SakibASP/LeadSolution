using Application.Interfaces.Menu;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace Lead.Api.Controllers.v1.Menu;

[ApiController]
[Route("api/v1/[controller]")]
public class MenuController(IMenuService menu) : Controller
{
    private readonly IMenuService _iMenu = menu;


    [Authorize]
    [HttpGet("get-menu-by-user")]
    public async Task<IActionResult> GetMenuList()
    {
        try
        {
            var name = User.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var menu = await _iMenu.GetAllMenuAsync(userId);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }
}
