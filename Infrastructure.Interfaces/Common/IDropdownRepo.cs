using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;

namespace Infrastructure.Interfaces.Common;

public interface IDropdownRepo
{
    Task<IList<DropdownDto>> GetDropdownListAsync(DropdownRequest request);
    Task<IList<UserDropdownDto>> GetUserDropdownListAsync(DropdownRequest request);
}
