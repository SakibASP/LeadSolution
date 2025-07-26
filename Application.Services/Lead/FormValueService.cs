using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Lead;

public class FormValueService(IGenericRepo<FormValues> repo) : IFormValueService
{
    private readonly IGenericRepo<FormValues> _iRepo = repo;
    public async Task<ApiResponse<dynamic>> AddAsync(FormValues formValues)
    {
        await _iRepo.AddAsync(formValues);
        return ApiResponse<dynamic>.Success(null, "Form value created successfully!");
    }

    public async Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? parameter)
    {
        var result = await _iRepo.GetAllAsync(parameter);
        return ApiResponse<IList<FormValues>>.Success(result, "Form values retrieved successfully!");
    }

    public async Task<ApiResponse<FormValues>> GetByIdAsync(int id)
    {
        var result = await _iRepo.GetByIdAsync(id);
        return ApiResponse<FormValues>.Success(result, "Form value retrieved successfully!");
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        await _iRepo.RemoveAsync(id);
        return ApiResponse<dynamic>.Success(null, "Form value removed successfully!");
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(FormValues formValues)
    {
        formValues.ModifiedDate = DateTime.Now.ToBangladeshTime();
        await _iRepo.UpdateAsync(formValues);
        return ApiResponse<dynamic>.Success(null, "Form value updated successfully!");
    }
}
