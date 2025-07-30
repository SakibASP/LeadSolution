using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IBusinessInfoService
{
    Task<ApiResponse<IList<AspNetBusinessInfo>>> GetAllAsync(dynamic? parameter);
    Task<ApiResponse<AspNetBusinessInfo>> GetByIdAsync(int id);
    Task<ApiResponse<dynamic>> AddAsync(AspNetBusinessInfoDto businessInfo);
    Task<ApiResponse<dynamic>> UpdateAsync(AspNetBusinessInfo businessInfo);
    Task<ApiResponse<dynamic>> RemoveAsync(int id);
}
