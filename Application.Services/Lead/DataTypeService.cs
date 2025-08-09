using Application.Interfaces.Lead;
using Common.Extentions;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class DataTypeService(IGenericRepo<DataTypes> repo, IHttpContextAccessor httpContext) : IDataTypeService
{
    private readonly IGenericRepo<DataTypes> _iDataType = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<dynamic>> AddAsync(DataTypes dataTypes)
    {
        try
        {
            dataTypes.CreatedBy = CurrentUser;
            await _iDataType.AddAsync(dataTypes);
            return ApiResponse<dynamic>.Success(true, "Data type created successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error creating DataType {dataTypes}", dataTypes);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<DataTypes>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iDataType.GetAllAsync(parameter);
            return ApiResponse<IList<DataTypes>>.Success(result, "Data type retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving DataTypes {parameter}", parameter);
            return ApiResponse<IList<DataTypes>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<DataTypes>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iDataType.GetByIdAsync(id);
            return ApiResponse<DataTypes>.Success(result, "Data type retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving DataType Id={Id}", id);
            return ApiResponse<DataTypes>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        try
        {
            await _iDataType.RemoveAsync(id);
            return ApiResponse<dynamic>.Success(true, "Data type removed successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error removing DataType Id={Id}", id);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(DataTypes dataTypes)
    {
        try
        {
            dataTypes.ModifiedBy = CurrentUser;
            dataTypes.ModifiedDate = DateTime.Now.ToBangladeshTime();
            await _iDataType.UpdateAsync(dataTypes);
            return ApiResponse<dynamic>.Success(true, "Data type updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating DataType {dataTypes}", dataTypes);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

}
