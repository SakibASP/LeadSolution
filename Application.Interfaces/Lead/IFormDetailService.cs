using Core.Models.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IFormDetailService
{
    Task<ApiResponse<IList<FormDetails>>> GetAllAsync(dynamic? parameter);
    Task<ApiResponse<FormDetails>> GetByIdAsync(int id);
    Task<ApiResponse<dynamic>> AddAsync(FormDetails formDetails, string userName);
    Task<ApiResponse<dynamic>> UpdateAsync(FormDetails formDetails, string userName);
    Task<ApiResponse<dynamic>> RemoveAsync(int id);
}
