using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _iFormValue.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IList<FormValues>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-dynamic-form")]
    public async Task<IActionResult> GetDynamicForm()
    {
        try
        {
            var result = await _iFormValue.GetDynamicFormAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<DynamicFormViewModel>.Fail("Something went wrong!"));
        }
    }


    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] DynamicFormViewModel formValues)
    {
        try
        {
            var result = await _iFormValue.AddAsync(formValues);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, formValues, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }
}
