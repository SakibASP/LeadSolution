using Application.Interfaces.Lead;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace Application.Services.Lead;

public class FormValueService(IFormValueRepo repo, IHttpContextAccessor httpContext) : IFormValueService
{
    private readonly IFormValueRepo _iFormValue = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";
    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";

    public async Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel viewModel)
    {
        try
        {
            await _iFormValue.AddAsync(viewModel);
            return ApiResponse<dynamic>.Success(true, "Messages added successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error adding DynamicForm: {@ViewModel}", JsonSerializer.Serialize(viewModel));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? param = null)
    {
        try
        {
            var result = await _iFormValue.GetAllAsync(param);
            return ApiResponse<IList<FormValues>>.Success(result, "Messages retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving messages with parameter: {@Param}", JsonSerializer.Serialize(param));
            return ApiResponse<IList<FormValues>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(int? businessId)
    {
        try
        {
            var result = await _iFormValue.GetDynamicFormAsync(businessId);
            return ApiResponse<DynamicFormViewModel>.Success(result, "Form values retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving dynamic form for BusinessId: {BusinessId}", businessId);
            return ApiResponse<DynamicFormViewModel>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> GetMessagesByBusinessAsync(GetFormValueRequest request)
    {
        try
        {
            var result = await _iFormValue.GetMessagesByBusinessAsync(request);
            return ApiResponse<dynamic>.Success(result, "Messages retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving messages for GetFormValueRequest: {request}", JsonSerializer.Serialize(request));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> UpdateFormSettingsAsync(UpdateFormSettingsRequest request)
    {
        try
        {
            request.Username = CurrentUser;
            await _iFormValue.UpdateFormSettingsAsync(request);
            return ApiResponse<dynamic>.Success(true, "Form updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating DynamicForm: {@ViewModel}", JsonSerializer.Serialize(request));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }
}

