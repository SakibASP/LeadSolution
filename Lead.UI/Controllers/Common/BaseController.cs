using Common.Extentions;
using Common.Utils.Constant;
using Common.Utils.Enums;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Common;

/// <summary>
/// Base controller for common authentication and token refresh logic. 
/// </summary>
public class BaseController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
{
    private readonly Dictionary<string, string> GetParam = [];
    /// <summary>
    /// HTTP service for making API requests.
    /// </summary>
    protected readonly IHttpService _httpService = httpService;

    /// <summary>
    /// API settings loaded from configuration.
    /// </summary>
    protected readonly ApiSettings _apiSettings = apiSetting.Value;

    /// <summary>
    /// Access token for the current session.
    /// </summary>
    protected string AccessToken { get; private set; } = string.Empty;

    /// <summary>
    /// Current Bangladesh Time
    /// </summary>
    protected DateTime CurrentBdTime { get; private set; } = DateTime.Now.ToBangladeshTime();

    /// <summary>
    /// Get logged users business List
    /// </summary>
    protected IList<DropdownDto>? UserBusinessList { get; private set; }

    /// <summary>
    /// Gets the utility controller endpoint from the API settings.
    /// </summary>
    protected string UtilityVersion => _apiSettings.Controllers.Utility;

    /// <summary>
    /// Gets the utility endpoints configuration for the API.
    /// </summary>
    protected UtilityEndpoints UtilityEndpoints => _apiSettings.Endpoints.Utility;

    /// <summary>
    /// Gets the common endpoints configuration used by the API.
    /// </summary>
    protected CommonEndpoints CommonEndpoints => _apiSettings.Endpoints.CommonEndPoints;

    /// <summary>
    /// Checks authentication and refreshes token if expired before executing an action.
    /// </summary>
    public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
    {
        // getting current auth session
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);

        // Check if the user is authenticated
        if (sessionAuth == null || string.IsNullOrEmpty(sessionAuth.Token))
        {
            // Redirect to the login page if session is missing or token is empty
            HttpContext.Response.Redirect(Constants.RedirectToLogin);
            return;
        }

        // Check if the token has been expired
        if (sessionAuth?.Expiration <= CurrentBdTime)
        {
            // Prepare token DTO for refresh request
            TokenDto tokenDto = new()
            {
                AccessToken = sessionAuth.Token,
                RefreshToken = sessionAuth.RefreshToken
            };

            // Call the API to refresh the token and get the response
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Controllers.Auth, _apiSettings.Endpoints.Auth.RefreshToken, tokenDto);

            // Check response status, if not success then redirect to login page
            if (!(response?.IsSuccess ?? false))
            {
                // Redirect to login page if token refresh fails
                HttpContext.Response.Redirect(Constants.RedirectToLogin);
                return;
            }

            // Store the refreshed authentication data in session
            sessionAuth = response?.Data;
            HttpContext.Session.Set(Constants.AuthResponseDto, sessionAuth);
        }

        // Set the access token for the current request
        AccessToken = sessionAuth?.Token ?? string.Empty;

        // Pass authentication data to the view
        ViewBag.Username = sessionAuth?.UserName ?? "";

        //Populating up user's businesslist variable
        var businessList = HttpContext.Session.Get<IList<DropdownDto>>(Constants.UserBusinessList);
        if (businessList is null)
        {
            GetParam.Clear();
            GetParam.Add("Id", ((int)DropdownEnum.UserWiseBusinesses).ToString());
            _httpService.SetBearerToken(AccessToken);
            var businessListResponse = await _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
                 UtilityVersion, UtilityEndpoints.GetDropdown, GetParam);
            UserBusinessList = businessListResponse?.Data;
            HttpContext.Session.Set(Constants.UserBusinessList, UserBusinessList);
        }
        else
        {
            UserBusinessList = businessList;
        }

        // Proceed with the action execution
        await next();
    }

    protected async Task<string> GetActiveApiKey(int businessId)
    {
        GetParam.Clear();
        GetParam.Add("businessId", businessId.ToString());
        var apiKeyTask = await _httpService.GetAsync<ApiResponse<string?>>(
            _apiSettings.Controllers.Auth,
            _apiSettings.Endpoints.Auth.GetApiKey,
            GetParam);
        return apiKeyTask?.Data ?? string.Empty;
    }
}
