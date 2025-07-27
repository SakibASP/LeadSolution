using Application.Interfaces.Lead;
using Core.Models.Lead;
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
public class FormDetailsController(IFormDetailService formDetail) : Controller
{
    private readonly IFormDetailService _iFormDetail = formDetail;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter) => Ok(await _iFormDetail.GetAllAsync(parameter));

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id) => Ok(await _iFormDetail.GetByIdAsync(id));

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] FormDetails formDetails) => Ok(await _iFormDetail.UpdateAsync(formDetails));

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] FormDetails formDetails) => Ok(await _iFormDetail.AddAsync(formDetails));

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id) => Ok(await _iFormDetail.RemoveAsync(id));
}
