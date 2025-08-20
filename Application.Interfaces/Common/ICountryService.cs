using Core.ViewModels.Response;

namespace Application.Interfaces.Common;

public interface ICountryService
{
    Task<ApiResponse<bool>> UpdateCountriesAsync();
}
