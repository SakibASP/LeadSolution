using Application.Interfaces.Lead;
using Core.Models.Lead;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Controllers.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 25 July 2025
/// </summary>

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class DataTypesController(IDataTypeService dataTypeService) : Controller
{
    private readonly IDataTypeService _iDataType = dataTypeService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter) => Ok(await _iDataType.GetAllAsync(parameter));

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id) => Ok(await _iDataType.GetByIdAsync(id));

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] DataTypes dataTypes) => Ok(await _iDataType.UpdateAsync(dataTypes));


    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] DataTypes dataTypes) => Ok(await _iDataType.AddAsync(dataTypes));

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id) => Ok(await _iDataType.RemoveAsync(id));

}
