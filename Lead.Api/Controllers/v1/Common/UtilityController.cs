using Application.Interfaces.Common;
using Core.ViewModels.Request.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Common;


[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UtilityController(IDropdownService service) : Controller
{
    private readonly IDropdownService _iDropService = service;

    [HttpGet("get-dropdown")]
    public async Task<IActionResult> GetDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetDropdownListAsync(request));
    
    [HttpGet("get-user-dropdown")]
    public async Task<IActionResult> GetUserDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetUserDropdownListAsync(request));
}
