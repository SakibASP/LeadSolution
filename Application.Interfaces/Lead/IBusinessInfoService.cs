using Core.Models.Auth;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IBusinessInfoService
{
    Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync(dynamic? parameter);
    Task<ApiResponse<AspNetBusinessInfo>> GetByIdAsync(int id);
    Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfo businessInfo, string userName);
    Task<ApiResponse<dynamic>> UpdateAsync(AspNetBusinessInfo businessInfo, string userName);
    Task<ApiResponse<dynamic>> RemoveAsync(int id);
}
