using Common.Extentions;
using Common.Utils.Constant;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lead.UI.Components;

public class MenuViewComponent(IHttpService httpService, IOptions<ApiSettings> apiSetting) : ViewComponent
{
    protected readonly IHttpService _httpService = httpService;
    protected readonly ApiSettings _apiSettings = apiSetting.Value;
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Remove any existing menu from session to ensure fresh load
        HttpContext.Session.Remove(Constants.Menu);

        // Retrieve authentication info from session
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);

        // Check if the user is authenticated
        if (sessionAuth == null || string.IsNullOrEmpty(sessionAuth.Token))
        {
            // Redirect to the login page if not authenticated
            HttpContext.Response.Redirect(Constants.RedirectToLogin);
        }

        // Check if the token is expired
        if (sessionAuth?.Expiration <= DateTime.Now.ToBangladeshTime())
        {
            // Prepare token DTO for refresh
            TokenDto tokenDto = new()
            {
                AccessToken = sessionAuth.Token,
                RefreshToken = sessionAuth.RefreshToken
            };

            // Attempt to refresh the token
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.RefreshToken, tokenDto);
            if (!(response?.IsSuccess ?? false))
            {
                // If the refresh token request fails, redirect to the login page
                HttpContext.Response.Redirect(Constants.RedirectToLogin);
            }

            // Update session with new authentication info
            sessionAuth = response?.Data;
            HttpContext.Session.Set(Constants.AuthResponseDto, sessionAuth);
        }

        // Set the bearer token for subsequent API calls
        _httpService.SetBearerToken(sessionAuth!.Token ?? "");

        // Fetch menu items for the authenticated user
        var data = await _httpService.GetAsync<ApiResponse<IList<DynamicMenuItemDto>>?>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.GetByUserId);
        var menuList = data?.Data ?? [];

        List<DynamicMenuItemDto>? _menuList;
        // Try to get menu from session
        var sessionMenu = HttpContext.Session.Get<IList<DynamicMenuItemDto>>(Constants.Menu);
        if (sessionMenu != null)
        {
            // Use menu from session if available
            _menuList = (List<DynamicMenuItemDto>?)sessionMenu;
        }
        else
        {
            // Otherwise, use freshly fetched menu and store in session
            _menuList = [.. menuList];
            HttpContext.Session.Set(Constants.Menu, menuList);
        }

        // Render the menu view with the menu list
        return View("_Menu", _menuList);
    }
}
