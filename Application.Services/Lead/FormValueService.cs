using Application.Interfaces.Lead;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Lead;

public class FormValueService(IFormValueRepo repo, IHttpContextAccessor httpContext) : IFormValueService
{
    private readonly IFormValueRepo _iRepo = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string? CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name;

    public async Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel viewModel)
    {
        try
        {
            await _iRepo.AddAsync(viewModel);
            return ApiResponse<dynamic>.Success(true, "Messages added successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error adding form value by user {CurrentUser}");
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? param = null)
    {
        try
        {
            var result = await _iRepo.GetAllAsync(param);
            return ApiResponse<IList<FormValues>>.Success(result, "Messages retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error retrieving all form values by user {CurrentUser}, Param: {param}");
            return ApiResponse<IList<FormValues>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(dynamic? param = null)
    {
        try
        {
            var result = await _iRepo.GetDynamicFormAsync(param);
            return ApiResponse<DynamicFormViewModel>.Success(result, "Form values retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error retrieving dynamic form by user {CurrentUser}, Param: {param}");
            return ApiResponse<DynamicFormViewModel>.Fail("Something went wrong!");
        }
    }
}

