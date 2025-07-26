using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 25 July 2025
/// </summary>
public class DataTypeService(IGenericRepo<DataTypes> repo) : IDataTypeService
{
    private readonly IGenericRepo<DataTypes> _iRepo = repo;
    public async Task<ApiResponse<dynamic>> AddAsync(DataTypes dataTypes, string userName)
    {
        dataTypes.CreatedBy = userName;
        await _iRepo.AddAsync(dataTypes);
        return ApiResponse<dynamic>.Success(null, "Data type created successfully!");
    }

    public async Task<ApiResponse<IList<DataTypes>>> GetAllAsync(dynamic? parameter)
    {
        var result = await _iRepo.GetAllAsync(parameter);
        return ApiResponse<IList<DataTypes>>.Success(result, "Data type retrieved successfully!");
    }

    public async Task<ApiResponse<DataTypes>> GetByIdAsync(int id)
    {
        var result = await _iRepo.GetByIdAsync(id);
        return ApiResponse<DataTypes>.Success(result, "Data type retrieved successfully!");
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        await _iRepo.RemoveAsync(id);
        return ApiResponse<dynamic>.Success(null, "Data type removed successfully!");
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(DataTypes dataTypes, string userName)
    {
        dataTypes.ModifiedBy = userName;
        dataTypes.ModifiedDate = DateTime.Now.ToBangladeshTime();
        await _iRepo.UpdateAsync(dataTypes);
        return ApiResponse<dynamic>.Success(null, "Data type updated successfully!");
    }
}
