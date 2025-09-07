using Application.Interfaces.Common;
using Common.Utils.Constant;
using Core.ViewModels.Request.Common;
using Lead.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UtilityController(IUtilityService utilityService) : Controller
{
    private readonly IUtilityService _iUtilityService = utilityService;

    [HttpGet("get-dropdown")]
    public async Task<IActionResult> GetDropdown([FromQuery] DropdownRequest request) => Ok(await _iUtilityService.GetDropdownListAsync(request));

    [HttpGet("get-user-dropdown")]
    public async Task<IActionResult> GetUserDropdown([FromQuery] DropdownRequest request) => Ok(await _iUtilityService.GetUserDropdownListAsync(request));

    [AllowAnonymous]
    [ApiKeyAuth]
    [HttpGet("get-client-dropdown")]
    public async Task<IActionResult> GetClientDropdown([FromQuery] DropdownRequest request) => Ok(await _iUtilityService.GetClientDropdownListAsync(request));
    
    [HttpPost("update-country-data")]
    public async Task<IActionResult> UpdateCountry() => Ok(await _iUtilityService.UpdateCountriesAsync());

    [HttpGet("get-system-logs")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetSystemLogs() => Ok(await _iUtilityService.GetSystemLogsAsync());

    [HttpGet("get-system-log-by-id")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetSystemLogById([FromQuery] int id) => Ok(await _iUtilityService.GetSystemLogByIdAsync(id));

    [HttpGet("get-api-logs")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetApiLogs() => Ok(await _iUtilityService.GetApiLogsAsync());

    [HttpGet("get-api-log-by-id")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetApiLogById([FromQuery] int id) => Ok(await _iUtilityService.GetApiLogByIdAsync(id));

}
