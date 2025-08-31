using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;

namespace Infrastructure.Interfaces.Lead;

public interface IFormValueRepo
{
    Task<IList<FormValues>> GetAllAsync(dynamic? param = null);
    Task<dynamic> GetMessagesByBusinessAsync(GetFormValueRequest request);
    Task<DynamicFormViewModel> GetDynamicFormAsync(int? businessId);
    Task AddAsync(DynamicFormViewModel formValues);
    Task UpdateFormSettingsAsync(UpdateFormSettingsRequest request);
}
