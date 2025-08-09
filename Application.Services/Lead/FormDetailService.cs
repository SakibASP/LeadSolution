using Application.Interfaces.Lead;
using Common.Extentions;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class FormDetailService(IGenericRepo<FormDetails> repo, IHttpContextAccessor httpContext) : IFormDetailService
{
    private readonly IGenericRepo<FormDetails> _iFormDetail = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<dynamic>> AddAsync(FormDetails formDetails)
    {
        try
        {
            formDetails.CreatedBy = CurrentUser;
            await _iFormDetail.AddAsync(formDetails);
            return ApiResponse<dynamic>.Success(true, "Form detail created successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error adding FormDetails: {@FormDetails}", formDetails);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormDetails>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iFormDetail.GetAllAsync(parameter);
            return ApiResponse<IList<FormDetails>>.Success(result, "Form details retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving all FormDetails with parameter: {@Parameter}", parameter);
            return ApiResponse<IList<FormDetails>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<FormDetails>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iFormDetail.GetByIdAsync(id);
            return ApiResponse<FormDetails>.Success(result, "Form detail retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving FormDetails by Id: {Id}", id);
            return ApiResponse<FormDetails>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        try
        {
            await _iFormDetail.RemoveAsync(id);
            return ApiResponse<dynamic>.Success(true, "Form detail removed successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error removing FormDetails by Id: {Id}", id);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }


    public async Task<ApiResponse<dynamic>> UpdateAsync(FormDetails formDetails)
    {
        try
        {
            formDetails.ModifiedBy = CurrentUser;
            formDetails.ModifiedDate = DateTime.Now.ToBangladeshTime();
            await _iFormDetail.UpdateAsync(formDetails);
            return ApiResponse<dynamic>.Success(true, "Form detail updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating FormDetails: {@FormDetails}", formDetails);
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }
}
