using Application.Interfaces.Menu;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Controllers.v1.Menu
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MenuController(IMenuService menu) : Controller
    {
        private readonly IMenuService _menu = menu;

        [Authorize]
        [HttpGet("get-menu-by-user")]
        public async Task<IActionResult> Index(string userId)
        {
            try
            {
                return Ok(await _menu.GetAllMenuAsync(userId));
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, userId, User.Identity?.Name));
                return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
            }
        }
    }
}
