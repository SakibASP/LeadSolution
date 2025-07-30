using Common.Utils.Enums;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Core.ViewModels.Response;

namespace Application.Interfaces.Common;

public interface IDropdownService
{
    Task<ApiResponse<IList<DropdownDto>>> GetDropdownListAsync(DropdownRequest request);
    Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request);
}
