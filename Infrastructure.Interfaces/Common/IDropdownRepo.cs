using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;

namespace Infrastructure.Interfaces.Common;

public interface IDropdownRepo
{
    Task<IList<T>> GetDropdownListAsync<T>(DropdownRequest request);
}
