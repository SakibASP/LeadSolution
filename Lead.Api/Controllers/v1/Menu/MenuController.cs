using Application.Interfaces.Menu;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace Lead.Api.Controllers.v1.Menu;

[ApiController]
[Route("api/v1/[controller]")]
public class MenuController(IMenuService menu, IHttpContextAccessor httpContext) : Controller
{
    private readonly IMenuService _iMenu = menu;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

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
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting menu");
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }
}
