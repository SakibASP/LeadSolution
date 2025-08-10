using Application.Interfaces.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 26 July 2025
/// </summary>

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class FormValuesController(IFormValueService formValue) : Controller
{
    private readonly IFormValueService _iFormValue = formValue;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll() => Ok(await _iFormValue.GetAllAsync());

    [HttpGet("get-dynamic-form")]
    public async Task<IActionResult> GetDynamicForm([FromQuery] int? businessId) => Ok(await _iFormValue.GetDynamicFormAsync(businessId));

    [HttpPost("update-form-settings")]
    public async Task<IActionResult> UpdateFormSettings([FromBody] UpdateFormSettingsRequest request) => Ok(await _iFormValue.UpdateFormSettingsAsync(request));

    [AllowAnonymous]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] DynamicFormViewModel formValues) => Ok(await _iFormValue.AddAsync(formValues));
}
