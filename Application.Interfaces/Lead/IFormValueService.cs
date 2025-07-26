using Core.Models.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IFormValueService
{
    Task<ApiResponse<IList<FormValues>>> GetAllAsync(dynamic? parameter);
    Task<ApiResponse<FormValues>> GetByIdAsync(int id);
    Task<ApiResponse<dynamic>> AddAsync(FormValues formValues);
    Task<ApiResponse<dynamic>> UpdateAsync(FormValues formValues);
    Task<ApiResponse<dynamic>> RemoveAsync(int id);
}
