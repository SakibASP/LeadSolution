using Application.Interfaces.Common;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace Application.Services.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>


public class CountryService(ICountryRepo repo, IHttpClientFactory factory, IHttpContextAccessor httpContext) : ICountryService
{
    private readonly ICountryRepo _iCountryRepo = repo;
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<bool>> UpdateCountriesAsync()
    {
        string response = string.Empty;
        try
        {
            const string url = "https://restcountries.com/v3.1/all?fields=name,flags,region,subregion,cca2,cca3,currencies";
            response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var countries = JsonSerializer.Deserialize<List<RestCountry>>(response, options);

            await _iCountryRepo.UpdateCountriesAsync(countries);

            return ApiResponse<bool>.Success(true, "Country refreshed successfully");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting country data: {@response}", response);
            return ApiResponse<bool>.Fail("Country refreshed failed");
        }

    }
}
