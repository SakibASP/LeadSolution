using Core.Models.Common;
using Core.ViewModels.Request.Common;

namespace Infrastructure.Interfaces.Common;

public interface IUtilityRepo
{
    Task<IList<T>> GetDropdownListAsync<T>(DropdownRequest request);
    Task UpdateCountriesAsync(IList<RestCountry>? restCountries);
    Task<IList<Logs>> GetSystemLogsAsync();
    Task<Logs> GetSystemLogByIdAsync(int id);
    Task<IList<RequestLogs>> GetApiLogsAsync();
    Task<RequestLogs> GetApiLogByIdAsync(int id);
    Task<int> GetDropdownIdByFormId(int formId);
}
