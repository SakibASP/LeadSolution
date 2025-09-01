using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace Application.Services.Lead;

public class FormDetailService(IGenericRepo<FormDetails> repo, IFormDetailRepo detailRepo, IHttpContextAccessor httpContext) : IFormDetailService
{
    private readonly IGenericRepo<FormDetails> _iGenericRepo = repo;
    private readonly IFormDetailRepo _iFormDetail = detailRepo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<dynamic>> AddAsync(FormDetails formDetails)
    {
        try
        {
            formDetails.CreatedBy = CurrentUser;
            await _iGenericRepo.AddAsync(formDetails);
            return ApiResponse<dynamic>.Success(true, "Form detail created successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error adding FormDetails: {@FormDetails}", JsonSerializer.Serialize(formDetails));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormDetails>>> GetAllAsync(dynamic? parameter)
    {
        try
        {
            var result = await _iGenericRepo.GetAllAsync(parameter);
            return ApiResponse<IList<FormDetails>>.Success(result, "Form details retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving all FormDetails with parameter: {@Parameter}", JsonSerializer.Serialize(parameter));
            return ApiResponse<IList<FormDetails>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<FormDetails>> GetByIdAsync(int id)
    {
        try
        {
            var result = await _iGenericRepo.GetByIdAsync(id);
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
            await _iGenericRepo.UpdateAsync(formDetails);
            return ApiResponse<dynamic>.Success(true, "Form detail updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating FormDetails: {@FormDetails}", JsonSerializer.Serialize(formDetails));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }
}
