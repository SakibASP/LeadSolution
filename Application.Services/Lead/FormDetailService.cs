using Application.Interfaces.Lead;
using Common.Extentions;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Lead;

public class FormDetailService(IGenericRepo<FormDetails> repo) : IFormDetailService
{
    private readonly IGenericRepo<FormDetails> _iRepo = repo;
    public async Task<ApiResponse<dynamic>> AddAsync(FormDetails formDetails, string userName)
    {
        formDetails.CreatedBy = userName;
        await _iRepo.AddAsync(formDetails);
        return ApiResponse<dynamic>.Success(null, "Form detail created successfully!");
    }

    public async Task<ApiResponse<IList<FormDetails>>> GetAllAsync(dynamic? parameter)
    {
        var result = await _iRepo.GetAllAsync(parameter);
        return ApiResponse<IList<FormDetails>>.Success(result, "Form details retrieved successfully!");
    }

    public async Task<ApiResponse<FormDetails>> GetByIdAsync(int id)
    {
        var result = await _iRepo.GetByIdAsync(id);
        return ApiResponse<FormDetails>.Success(result, "Form detail retrieved successfully!");
    }

    public async Task<ApiResponse<dynamic>> RemoveAsync(int id)
    {
        await _iRepo.RemoveAsync(id);
        return ApiResponse<dynamic>.Success(null, "Form detail removed successfully!");
    }

    public async Task<ApiResponse<dynamic>> UpdateAsync(FormDetails formDetails, string userName)
    {
        formDetails.ModifiedBy = userName;
        formDetails.ModifiedDate = DateTime.Now.ToBangladeshTime();
        await _iRepo.UpdateAsync(formDetails);
        return ApiResponse<dynamic>.Success(null, "Form detail updated successfully!");
    }
}
