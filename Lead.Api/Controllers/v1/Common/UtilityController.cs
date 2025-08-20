using Application.Interfaces.Common;
using Core.ViewModels.Request.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>


[ApiController]
[Route("api/v1/[controller]")]
public class UtilityController(IDropdownService drpService,ICountryService countryService) : Controller
{
    private readonly IDropdownService _iDropService = drpService;
    private readonly ICountryService _iCountryService = countryService;

    [Authorize]
    [HttpGet("get-dropdown")]
    public async Task<IActionResult> GetDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetDropdownListAsync(request));

    [Authorize]
    [HttpGet("get-user-dropdown")]
    public async Task<IActionResult> GetUserDropdown([FromQuery] DropdownRequest request) => Ok(await _iDropService.GetUserDropdownListAsync(request));
    
    [Authorize]
    [HttpGet("update-country-data")]
    public async Task<IActionResult> UpdateCountry([FromQuery] DropdownRequest request) => Ok(await _iCountryService.UpdateCountriesAsync());
}
