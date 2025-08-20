namespace Infrastructure.Interfaces.Common;

public interface ICountryRepo
{
    Task UpdateCountriesAsync(IList<RestCountry>? restCountries);
}
