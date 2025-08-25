using Application.Interfaces.Common;
using Common.Utils.Constant;
using Core.ViewModels.Request.Common;
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
    
    [HttpPost("update-country-data")]
    public async Task<IActionResult> UpdateCountry() => Ok(await _iUtilityService.UpdateCountriesAsync());

    [HttpGet("get-system-logs")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetLogs() => Ok(await _iUtilityService.GetLogsAsync());

    [HttpGet("get-system-log-by-id")]
    [Authorize(Roles = Constants.SuperAdmin)]
    public async Task<IActionResult> GetLogById([FromQuery] int id) => Ok(await _iUtilityService.GetLogsByIdAsync(id));

}
