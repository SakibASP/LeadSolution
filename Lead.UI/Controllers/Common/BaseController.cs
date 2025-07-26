using Common.Extentions;
using Common.Utils.Constant;
using Core.ViewModels.Dto.Auth;
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
    protected string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Current Bangladesh Time
    /// </summary>
    protected DateTime CurrentBdTime { get; set; } = DateTime.Now.ToBangladeshTime();

    /// <summary>
    /// Checks authentication and refreshes token if expired before executing an action.
    /// </summary>
    /// <param name="filterContext">The action executing context.</param>
    /// <param name="next">Delegate to execute the next action.</param>
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
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.RefreshToken, tokenDto);

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
        ViewBag.AuthResponseDto = sessionAuth;

        // Proceed with the action execution
        await next();
    }
}
