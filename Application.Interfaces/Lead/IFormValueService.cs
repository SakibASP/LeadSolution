using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IFormValueService
{
    Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? param = null);
    Task<ApiResponse<dynamic>> GetMessagesByBusinessAsync(int businessId);
    Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(int? businessId);
    Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel formValues);
    Task<ApiResponse<dynamic>> UpdateFormSettingsAsync(UpdateFormSettingsRequest request);
}
