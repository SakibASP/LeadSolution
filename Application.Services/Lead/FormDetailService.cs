using Application.Interfaces.Lead;
using Common.Extentions;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class FormDetailService(IGenericRepo<FormDetails> repo, IHttpContextAccessor httpContext) : IFormDetailService
{
    private readonly IGenericRepo<FormDetails> _iRepo = repo;
    private readonly IHttpContextAccessor _httpContext= httpContext;
    public async Task<ApiResponse<dynamic>> AddAsync(FormDetails formDetails)
    {
        try
        {
            formDetails.CreatedBy = _httpContext.HttpContext!.User.Identity?.Name;
            await _iRepo.AddAsync(formDetails);
            return ApiResponse<dynamic>.Success(null, "Form detail created successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(_httpContext.HttpContext!.Request.Path, formDetails, _httpContext.HttpContext.User.Identity?.Name));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormDetails>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iRepo.GetAllAsync(parameter);
            return ApiResponse<IList<FormDetails>>.Success(result, "Form details retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(_httpContext.HttpContext!.Request.Path, parameter, _httpContext.HttpContext.User.Identity?.Name));
            return ApiResponse<IList<FormDetails>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<FormDetails>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iRepo.GetByIdAsync(id);
            return ApiResponse<FormDetails>.Success(result, "Form detail retrieved successfully!");
        }
        catch(Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(_httpContext.HttpContext!.Request.Path, id, _httpContext.HttpContext.User.Identity?.Name));
            return ApiResponse<FormDetails>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        try
        {
            await _iRepo.RemoveAsync(id);
            return ApiResponse<dynamic>.Success(null, "Form detail removed successfully!");
        }
        catch(Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(_httpContext.HttpContext!.Request.Path, id, _httpContext.HttpContext.User.Identity?.Name));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(FormDetails formDetails)
    {
        try
        {
            formDetails.ModifiedBy = _httpContext.HttpContext!.User.Identity?.Name;
            formDetails.ModifiedDate = DateTime.Now.ToBangladeshTime();
            await _iRepo.UpdateAsync(formDetails);
            return ApiResponse<dynamic>.Success(null, "Form detail updated successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(_httpContext.HttpContext!.Request.Path, formDetails, _httpContext.HttpContext.User.Identity?.Name));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }
}
