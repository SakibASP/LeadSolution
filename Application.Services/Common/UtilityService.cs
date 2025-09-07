using Application.Interfaces.Common;
using Azure.Core;
using Common.Utils.Constant;
using Common.Utils.Enums;
using Core.Models.Common;
using Core.ViewModels.Dto.Auth.Auth;
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

    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public UserInfoViewModel UserInfo()
    {
        return new()
        {
            UserId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = _httpContext.HttpContext?.User.Identity?.Name,
            Email = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Email),
            UserRoles = _httpContext.HttpContext?.User
                .FindAll(ClaimTypes.Role)
                .Select(r => r.Value)
                .ToList() ?? [],
            UserBusinessList = [],
            AccessToken = ""
        };
    }

    public async Task<ApiResponse<IList<DropdownDto>>> GetDropdownListAsync(DropdownRequest request)
    {
        try
        {
            bool IsAdmin = UserInfo().UserRoles?.Contains(Constants.Admin) is true || UserInfo().UserRoles?.Contains(Constants.SuperAdmin) is true;
            // if the dropdown is 'user wise business' then it takes the related dropdown using the user id
            if (request.Id == (int)DropdownEnum.UserWiseBusinesses && !IsAdmin)
                request.Param1 = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _iUtilityRepo.GetDropdownListAsync<DropdownDto>(request);
            return ApiResponse<IList<DropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<DropdownDto>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request)
    {
        try
        {
            bool IsAdmin = UserInfo().UserRoles?.Contains(Constants.Admin) is true || UserInfo().UserRoles?.Contains(Constants.SuperAdmin) is true;
            if (!IsAdmin)
                request.Param1 = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _iUtilityRepo.GetDropdownListAsync<UserDropdownDto>(request);
            return ApiResponse<IList<UserDropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<UserDropdownDto>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<DropdownDto>>> GetClientDropdownListAsync(DropdownRequest request)
    {
        try
        {
            request.Id = await _iUtilityRepo.GetDropdownIdByFormId(request.Id);
            var result = await _iUtilityRepo.GetDropdownListAsync<DropdownDto>(request);
            return ApiResponse<IList<DropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@formId}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<DropdownDto>>.Fail("Something went wrong!");
        }
    }

    #region - country api -
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
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting country data: {@response}", response);
            return ApiResponse<bool>.Fail("Country refreshed failed");
        }

    }
    #endregion

    #region - logs -
    public async Task<ApiResponse<IList<Logs>>> GetSystemLogsAsync()
    {
        try
        {
            var result = await _iUtilityRepo.GetSystemLogsAsync();
            return ApiResponse<IList<Logs>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting system logs");
            return ApiResponse<IList<Logs>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<Logs>> GetSystemLogByIdAsync(int id)
    {
        try
        {
            var result = await _iUtilityRepo.GetSystemLogByIdAsync(id);
            return ApiResponse<Logs>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting system log detail {id}", id);
            return ApiResponse<Logs>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<RequestLogs>>> GetApiLogsAsync()
    {
        try
        {
            var result = await _iUtilityRepo.GetApiLogsAsync();
            return ApiResponse<IList<RequestLogs>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting api logs");
            return ApiResponse<IList<RequestLogs>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<RequestLogs>> GetApiLogByIdAsync(int id)
    {
        try
        {
            var result = await _iUtilityRepo.GetApiLogByIdAsync(id);
            return ApiResponse<RequestLogs>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", UserInfo().UserName)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting api log detail {id}", id);
            return ApiResponse<RequestLogs>.Fail("Something went wrong!");
        }
    }
    #endregion
}
