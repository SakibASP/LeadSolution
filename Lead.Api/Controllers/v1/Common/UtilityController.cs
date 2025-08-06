using Application.Interfaces.Common;
using Core.ViewModels.Request.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Common;


[ApiController]
[Route("api/v1/[controller]")]
public class UtilityController(IDropdownService service) : Controller
{
    private readonly IDropdownService _iDropService = service;

    [Authorize]
    [HttpGet("get-dropdown")]
    public async Task<IActionResult> GetDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetDropdownListAsync(request));

    [Authorize]
    [HttpGet("get-user-dropdown")]
    public async Task<IActionResult> GetUserDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetUserDropdownListAsync(request));
}
