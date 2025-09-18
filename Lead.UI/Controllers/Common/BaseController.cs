using Common.Utils.Constant;
using Common.Utils.Enums;
using Common.Utils.Extentions;
using Common.Utils.Extentions;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    /// Current User's Information
    /// </summary>
    protected UserInfoViewModel UserInfo { get; private set; } = default!;


    /// <summary>
    /// Current Bangladesh Time
    /// </summary>
    /// 
    protected DateTime CurrentBdTime { get; private set; } = DateTime.Now.ToBangladeshTime();


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
            HttpContext.Session.Clear();
            HttpContext.Response.Redirect(Constants.RedirectToLogin);
            return;
        }

        // Check if the token has been expired
        if (sessionAuth?.Expiration <= CurrentBdTime)
        {
            TokenDto tokenDto = new()
            {
                AccessToken = sessionAuth.Token,
                RefreshToken = sessionAuth.RefreshToken
            };

            // Call the API to refresh the token and get the response
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Controllers.Auth, _apiSettings.Endpoints.Auth.RefreshToken, tokenDto);

            // Check response status, if not success then redirect to login page
            if (response?.IsSuccess is not true)
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Redirect(Constants.RedirectToLogin);
                return;
            }

            sessionAuth = response?.Data;
            HttpContext.Session.Set(Constants.AuthResponseDto, sessionAuth);
        }

        //populate user info
        var userInfo = HttpContext.Session.Get<UserInfoViewModel>(Constants.UserInfo);
        if (userInfo is null)
        {
            if (!string.IsNullOrEmpty(sessionAuth?.Token))
            {
                JwtSecurityTokenHandler handler = new();
                UserInfo = new UserInfoViewModel
                {
                    AccessToken = sessionAuth.Token
                };
                var jsonToken = handler.ReadToken(UserInfo.AccessToken) as JwtSecurityToken;

                UserInfo.UserName = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                UserInfo.UserId = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                UserInfo.Email = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                UserInfo.UserRoles = jsonToken?.Claims
                                     .Where(c => c.Type == ClaimTypes.Role)
                                     .Select(c => c.Value)
                                     .ToList();
                GetParam.Clear();
                GetParam.Add("Id", ((int)DropdownEnum.UserWiseBusinesses).ToString());
                _httpService.SetBearerToken(UserInfo.AccessToken);
                var businessListResponse = await _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
                     UtilityVersion, UtilityEndpoints.GetDropdown, GetParam);

                UserInfo.UserBusinessList = businessListResponse?.Data;
                HttpContext.Session.Set(Constants.UserInfo, UserInfo);
            }
        }
        else
        {
            UserInfo = userInfo;
        }

        // User Rights Checking
        var sessionMenu = HttpContext.Session.Get<IList<DynamicMenuItemDto>>(Constants.Menu);
        if (sessionMenu != null)
        {
            var menu = (IEnumerable<DynamicMenuItemDto>)sessionMenu;
            var executionPath = Request.Path.ToString();
            if (!string.IsNullOrEmpty(executionPath))
            {
                if (UserInfo.UserRoles?.Contains(Constants.AdminAuthRoles) is false && IsRestricted(executionPath))
                {
                    var rights = menu.Where(h => h.MenuURL != "#").Where(p => p.MenuURL!.Split('/')[1] == executionPath.Split('/')[1]).ToList();
                    if (rights.Count > 0)
                    {
                        //Do nothing
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        HttpContext.Response.Redirect(Constants.RedirectToLogin);
                        return;
                    }

                }
            }
        }

        // Redirect to business create page if no business found
        if (UserInfo.UserBusinessList?.Count <= 0 && UserInfo.UserRoles!.Contains(Constants.Client))
            if (!HttpContext.Request.Path.Equals(Constants.RedirectToBusinessCreate, StringComparison.OrdinalIgnoreCase))
                HttpContext.Response.Redirect(Constants.RedirectToBusinessCreate);


        // Pass authentication data to the view
        ViewBag.Username = UserInfo.UserName;
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

    private static bool IsRestricted(string executionPath)
    {
        Span<string> permittedMethods = ["UserProfile", "UpdateProfile", "ChangePassword"];
        string[] paths = executionPath.Split('/');
        if (paths.Length > 2)
            if (permittedMethods.Contains(paths[2])) 
                return false;

        Span<string> permittedContriollers = ["Home", "Dashboard"];
        if (!executionPath.Equals("/") && !permittedContriollers.Contains(executionPath.Split('/')[1])) 
            return true;
        else 
            return false;
    }
}
