using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Controllers.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 26 July 2025
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class FormValuesController(IFormValueService formValue) : Controller
{
    private readonly IFormValueService _iFormValue = formValue;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter)
    {
        try
        {
            var result = await _iFormValue.GetAllAsync(parameter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, parameter, User.Identity?.Name));
            return Ok(ApiResponse<IList<FormValues>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var result = await _iFormValue.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<int>.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<FormValues>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] FormValues formValues)
    {
        try
        {
            var result = await _iFormValue.UpdateAsync(formValues);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<FormValues>.GenerateErrorMsg(HttpContext.Request.Path, formValues, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] FormValues formValues)
    {
        try
        {
            var result = await _iFormValue.AddAsync(formValues);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<FormValues>.GenerateErrorMsg(HttpContext.Request.Path, formValues, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id)
    {
        try
        {
            var result = await _iFormValue.RemoveAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<int>.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }
}
