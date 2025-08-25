using Application.Interfaces.Common;
using Azure.Core;
using Common.Utils.Enums;
using Core.Models.Common;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Services.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>


public class UtilityService(IUtilityRepo repo, IHttpClientFactory factory, IHttpContextAccessor httpContext) : IUtilityService
{
    private readonly IUtilityRepo _iUtilityRepo = repo;
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<IList<DropdownDto>>> GetDropdownListAsync(DropdownRequest request)
    {
        try
        {
            // if the dropdown is 'user wise business' then it takes the related dropdown using the user id
            if (request.Id == (int)DropdownEnum.UserWiseBusinesses)
                request.Param1 = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _iUtilityRepo.GetDropdownListAsync<DropdownDto>(request);
            return ApiResponse<IList<DropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<DropdownDto>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request)
    {
        try
        {
            var result = await _iUtilityRepo.GetDropdownListAsync<UserDropdownDto>(request);
            return ApiResponse<IList<UserDropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<UserDropdownDto>>.Fail("Something went wrong!");
        }
    }

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

            await _iUtilityRepo.UpdateCountriesAsync(countries);

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

    public async Task<ApiResponse<IList<Logs>>> GetLogsAsync()
    {
        try
        {
            var result = await _iUtilityRepo.GetLogsAsync();
            return ApiResponse<IList<Logs>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting logs");
            return ApiResponse<IList<Logs>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<Logs>> GetLogsByIdAsync(int id)
    {
        try
        {
            var result = await _iUtilityRepo.GetLogsByIdAsync(id);
            return ApiResponse<Logs>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting log detail {id}", id);
            return ApiResponse<Logs>.Fail("Something went wrong!");
        }
    }
}
