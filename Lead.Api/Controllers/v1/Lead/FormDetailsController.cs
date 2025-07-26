using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Controllers.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 26 July 2025
/// </summary>

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class FormDetailsController(IFormDetailService formDetail) : Controller
{
    private readonly IFormDetailService _iFormDetail = formDetail;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter)
    {
        try
        {
            var result = await _iFormDetail.GetAllAsync(parameter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, parameter, User.Identity?.Name));
            return Ok(ApiResponse<IList<FormDetails>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var result = await _iFormDetail.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<int>.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<FormDetails>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] FormDetails formDetails)
    {
        try
        {
            var result = await _iFormDetail.UpdateAsync(formDetails, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<FormDetails>.GenerateErrorMsg(HttpContext.Request.Path, formDetails, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] FormDetails formDetails)
    {
        try
        {
            var result = await _iFormDetail.AddAsync(formDetails, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<FormDetails>.GenerateErrorMsg(HttpContext.Request.Path, formDetails, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id)
    {
        try
        {
            var result = await _iFormDetail.RemoveAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<int>.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }
}
