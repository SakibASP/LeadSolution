using Core.Models.Common;
using Core.ViewModels.Request.Common;

namespace Infrastructure.Interfaces.Common;

public interface IUtilityRepo
{
    Task<IList<T>> GetDropdownListAsync<T>(DropdownRequest request);
    Task UpdateCountriesAsync(IList<RestCountry>? restCountries);
    Task<IList<Logs>> GetLogsAsync();
    Task<Logs> GetLogsByIdAsync(int id);
}
