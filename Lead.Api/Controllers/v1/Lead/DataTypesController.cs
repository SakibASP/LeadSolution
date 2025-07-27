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
/// 25 July 2025
/// </summary>

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class DataTypesController(IDataTypeService dataTypeService) : Controller
{
    private readonly IDataTypeService _iDataType = dataTypeService;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] dynamic? parameter)
    {
        try
        {
            var result = await _iDataType.GetAllAsync(parameter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, parameter, User.Identity?.Name));
            return Ok(ApiResponse<IList<DataTypes>>.Fail("Something went wrong!"));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var result = await _iDataType.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<DataTypes>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] DataTypes dataTypes)
    {
        try
        {
            var result = await _iDataType.UpdateAsync(dataTypes, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, dataTypes, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] DataTypes dataTypes)
    {
        try
        {
            var result = await _iDataType.AddAsync(dataTypes, User.Identity?.Name ?? "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, dataTypes, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] int id)
    {
        try
        {
            var result = await _iDataType.RemoveAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, id, User.Identity?.Name));
            return Ok(ApiResponse<dynamic>.Fail("Something went wrong!"));
        }
    }
}
