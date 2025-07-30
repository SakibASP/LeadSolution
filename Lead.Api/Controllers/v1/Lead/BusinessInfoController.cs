using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// Md. Sakibur Rahman
/// 27 July 2025
/// </summary>
namespace Lead.Api.Controllers.v1.Lead;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class BusinessInfoController(IBusinessInfoService businessInfoService) : Controller
{
    private readonly IBusinessInfoService _iBusinessInfo = businessInfoService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter) => Ok(await _iBusinessInfo.GetAllAsync(parameter));

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id) => Ok(await _iBusinessInfo.GetByIdAsync(id));

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AspNetBusinessInfoDto businessInfo) => Ok(await _iBusinessInfo.AddAsync(businessInfo));

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] AspNetBusinessInfo businessInfo) => Ok(await _iBusinessInfo.UpdateAsync(businessInfo));

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id) => Ok(await _iBusinessInfo.RemoveAsync(id));
}

