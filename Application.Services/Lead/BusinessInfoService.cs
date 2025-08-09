using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class BusinessInfoService(
    IGenericRepo<AspNetBusinessInfo> genericInfo,
    IBusinessInfoRepo businessInfo,
    IHttpContextAccessor httpContext) : IBusinessInfoService
{
    private readonly IGenericRepo<AspNetBusinessInfo> _iGenericRepo = genericInfo;
    private readonly IBusinessInfoRepo _iBusinessInfo = businessInfo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iGenericRepo.GetAllAsync(parameter);
            return ApiResponse<IList<AspNetBusinessInfo>>.Success(result, "Business list retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving business list with parameter: {@Parameter}", parameter);
            return ApiResponse<IList<AspNetBusinessInfo>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<AspNetBusinessInfo>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iGenericRepo.GetByIdAsync(id);
            return ApiResponse<AspNetBusinessInfo>.Success(result, "Business info retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving business info by Id: {Id}", id);
            return ApiResponse<AspNetBusinessInfo>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfoDto businessInfo)
    {
        try
        {
            businessInfo.UserName = CurrentUser;
            await _iBusinessInfo.AddAsync(businessInfo);
            return ApiResponse<dynamic>.Success(true, "Business info created successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error creating business info: {@BusinessInfo}", businessInfo);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(AspNetBusinessInfo businessInfo)
    {
        try
        {
            businessInfo.ModifiedBy = CurrentUser;
            businessInfo.ModifiedDate = DateTime.Now.ToBangladeshTime();
            await _iBusinessInfo.UpdateAsync(businessInfo);
            return ApiResponse<dynamic>.Success(true, "Business info updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating business info: {@BusinessInfo}", businessInfo);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        try
        {
            await _iGenericRepo.RemoveAsync(id);
            return ApiResponse<dynamic>.Success(true, "Business info removed successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error removing business info by Id: {Id}", id);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

}
