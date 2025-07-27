using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Auth;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Lead;

public class BusinessInfoService(IGenericRepo<AspNetBusinessInfo> repo) : IBusinessInfoService
{
    private readonly IGenericRepo<AspNetBusinessInfo> _iRepo = repo;

    public async Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync(dynamic? parameter)
    {
        var result = await _iRepo.GetAllAsync(parameter);
        return ApiResponse<IList<AspNetBusinessInfo>>.Success(result, "Business list retrieved successfully!");
    }

    public async Task<ApiResponse<AspNetBusinessInfo>> GetByIdAsync(int id)
    {
        var result = await _iRepo.GetByIdAsync(id);
        return ApiResponse<AspNetBusinessInfo>.Success(result, "Business info retrieved successfully!");
    }

    public async Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfo businessInfo, string userName)
    {
        businessInfo.CreatedBy = userName;
        await _iRepo.AddAsync(businessInfo);
        return ApiResponse<dynamic>.Success(null, "Business info created successfully!");
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(AspNetBusinessInfo businessInfo, string userName)
    {
        businessInfo.ModifiedBy = userName;
        businessInfo.ModifiedDate = DateTime.Now.ToBangladeshTime();
        await _iRepo.UpdateAsync(businessInfo);
        return ApiResponse<dynamic>.Success(null, "Business info updated successfully!");
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        await _iRepo.RemoveAsync(id);
        return ApiResponse<dynamic>.Success(null, "Business info removed successfully!");
    }
}
