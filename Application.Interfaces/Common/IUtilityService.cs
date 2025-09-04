using Core.Models.Common;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Core.ViewModels.Response;

namespace Application.Interfaces.Common;

public interface IUtilityService
{
    UserInfoViewModel UserInfo();
    Task<ApiResponse<IList<DropdownDto>>> GetDropdownListAsync(DropdownRequest request);
    Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request);
    Task<ApiResponse<bool>> UpdateCountriesAsync();
    Task<ApiResponse<IList<Logs>>> GetSystemLogsAsync();
    Task<ApiResponse<Logs>> GetSystemLogByIdAsync(int id);
    Task<ApiResponse<IList<RequestLogs>>> GetApiLogsAsync();
    Task<ApiResponse<RequestLogs>> GetApiLogByIdAsync(int id);
}
