using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IFormValueService
{
    Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? param = null);
    Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(dynamic? param = null);
    Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel formValues);
}
