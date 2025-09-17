using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IFormValueService
{
    //Task<ApiResponse<dynamic>> GetMessagesByBusinessAsync(GetFormValueRequest request);
    Task<ApiResponse<IList<FormValueMaster>>> GetMessagesByBusinessAsync(GetFormValueRequest request);
    Task<ApiResponse<IList<FormValueViewModel>>> GetMessagesDetailsByMasterIdAsync(int masterId);
    Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(int? businessId);
    Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel formValues);
    Task<ApiResponse<dynamic>> UpdateFormSettingsAsync(UpdateFormSettingsRequest request);
}
