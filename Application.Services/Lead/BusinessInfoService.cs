using Application.Interfaces.Lead;
using Common.Extentions;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class BusinessInfoService(
    IGenericRepo<AspNetBusinessInfo> repo,
    IServiceTypeRepo serviceType,
    IHttpContextAccessor httpContext) : IBusinessInfoService
{
    private readonly IGenericRepo<AspNetBusinessInfo> _iBusinessInfo = repo;
    private readonly IServiceTypeRepo _iServiceTypes = serviceType;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iBusinessInfo.GetAllAsync(parameter);
            return ApiResponse<IList<AspNetBusinessInfo>>.Success(result, "Business list retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, parameter, CurrentUser));
            return ApiResponse<IList<AspNetBusinessInfo>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<AspNetBusinessInfo>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iBusinessInfo.GetByIdAsync(id);
            return ApiResponse<AspNetBusinessInfo>.Success(result, "Business info retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, id, CurrentUser));
            return ApiResponse<AspNetBusinessInfo>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfo businessInfo)
    {
        try
        {
            businessInfo.CreatedBy = CurrentUser;
            await _iBusinessInfo.AddAsync(businessInfo);
            return ApiResponse<dynamic>.Success(true, "Business info created successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, businessInfo, CurrentUser));
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
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, businessInfo, CurrentUser));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        try
        {
            await _iBusinessInfo.RemoveAsync(id);
            return ApiResponse<dynamic>.Success(true, "Business info removed successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, id, CurrentUser));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<AspNetServiceTypes>>> GetAllServiceTypesAsync()
    {
        try
        {
            var result = await _iServiceTypes.GetAllAsync();
            return ApiResponse<IList<AspNetServiceTypes>>.Success(result, "Service types retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, null, CurrentUser));
            return ApiResponse<IList<AspNetServiceTypes>>.Fail("Something went wrong!");
        }
    }
}
