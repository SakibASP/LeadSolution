using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;

namespace Infrastructure.Interfaces.Lead;

public interface IFormValueRepo
{
    Task<IList<FormValues>> GetAllAsync(dynamic? param = null);
    Task<DynamicFormViewModel> GetDynamicFormAsync(int? businessId);
    Task AddAsync(DynamicFormViewModel formValues);
}
