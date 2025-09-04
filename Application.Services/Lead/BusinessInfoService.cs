using Application.Interfaces.Common;
using Application.Interfaces.Lead;
using Azure.Core;
using Common.Extentions;
using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Dynamic;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Services.Lead;

public class BusinessInfoService(
    IGenericRepo<AspNetBusinessInfo> genericInfo,
    IBusinessInfoRepo businessInfo,
    IUtilityService utility,
    IHttpContextAccessor httpContext) : IBusinessInfoService
{
    private readonly IGenericRepo<AspNetBusinessInfo> _iGenericRepo = genericInfo;
    private readonly IBusinessInfoRepo _iBusinessInfo = businessInfo;
    private readonly IUtilityService _iUtility = utility;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private UserInfoViewModel UserInfo => _iUtility.UserInfo();
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync()
    {
        try
        {
            var result = await _iBusinessInfo.GetAsync(UserInfo);
            return ApiResponse<IList<AspNetBusinessInfo>>.Success(result, "Business list retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo.UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving business list with parameter: {@Parameter}", JsonSerializer.Serialize(UserInfo));
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
                .ForContext("UserName", UserInfo.UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving business info by Id: {Id}", id);
            return ApiResponse<AspNetBusinessInfo>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfoDto businessInfo)
    {
        try
        {
            businessInfo.UserName = UserInfo.UserName;
            await _iBusinessInfo.AddAsync(businessInfo);
            return ApiResponse<dynamic>.Success(true, "Business info created successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo.UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error creating business info: {@BusinessInfo}", JsonSerializer.Serialize(businessInfo));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(AspNetBusinessInfo businessInfo)
    {
        try
        {
            businessInfo.ModifiedBy = UserInfo.UserName;
            businessInfo.ModifiedDate = DateTime.Now.ToBangladeshTime();
            await _iGenericRepo.UpdateAsync(businessInfo);
            return ApiResponse<dynamic>.Success(true, "Business info updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo.UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating business info: {@BusinessInfo}", JsonSerializer.Serialize(businessInfo));
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
                .ForContext("UserName", UserInfo.UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error removing business info by Id: {Id}", id);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

}
