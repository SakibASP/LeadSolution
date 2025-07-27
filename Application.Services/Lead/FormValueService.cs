using Application.Interfaces.Lead;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Lead;

public class FormValueService(IFormValueRepo repo) : IFormValueService
{
    private readonly IFormValueRepo _iRepo = repo;
    public async Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel viewModel)
    {
        await _iRepo.AddAsync(viewModel);
        return ApiResponse<dynamic>.Success(null, "Messages added successfully!");
    }

    public async Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? param = null)
    {
        var result =  await _iRepo.GetAllAsync(param);
        return ApiResponse<IList<FormValues>>.Success(result, "Messages get successfully!");
    }

    public async Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(dynamic? param = null)
    {
        var result = await _iRepo.GetDynamicFormAsync(param);
        return ApiResponse<DynamicFormViewModel>.Success(result, "Form values retrieved successfully!");
    }
}
