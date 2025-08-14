using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;

namespace Infrastructure.Interfaces.Lead;

public interface IFormValueRepo
{
    Task<IList<FormValues>> GetAllAsync(dynamic? param = null);
    Task<dynamic> GetMessagesByBusinessAsync(int businessId);
    Task<DynamicFormViewModel> GetDynamicFormAsync(int? businessId);
    Task AddAsync(DynamicFormViewModel formValues);
    Task UpdateFormSettingsAsync(UpdateFormSettingsRequest request);
}
