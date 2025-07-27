using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// Md. Sakibur Rahman
/// 27 July 2025
/// </summary>
namespace Lead.Api.Controllers.v1.Lead;


[ApiController]
[Route("api/v1/[controller]")]
public class BusinessInfoController(IBusinessInfoService businessInfoService) : Controller
{
    private readonly IBusinessInfoService _iBusinessInfo = businessInfoService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter)
    {
        try
        {
            var result = await _iBusinessInfo.GetAllAsync(parameter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, parameter, User.Identity?.Name));
            return Ok(ApiResponse<IList<AspNetBusinessInfo>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var result = await _iBusinessInfo.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<AspNetBusinessInfo>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AspNetBusinessInfo businessInfo)
    {
        try
        {
            var result = await _iBusinessInfo.AddAsync(businessInfo, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, businessInfo, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] AspNetBusinessInfo businessInfo)
    {
        try
        {
            var result = await _iBusinessInfo.UpdateAsync(businessInfo, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, businessInfo, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [Authorize]
    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id)
    {
        try
        {
            var result = await _iBusinessInfo.RemoveAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }
}

