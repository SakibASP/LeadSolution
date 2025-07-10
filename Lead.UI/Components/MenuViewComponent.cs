using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth;
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
        HttpContext.Session.Remove(Constants.Menu);
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);

        // Check if the user is authenticated
        if (sessionAuth == null || string.IsNullOrEmpty(sessionAuth.Token))
        {
            //Redirect to the login page
            HttpContext.Response.Redirect(Constants.RedirectToLogin);
        }

        if (sessionAuth?.Expiration <= TimeHelper.GetCurrentBangladeshTime())
        {
            TokenDto tokenDto = new()
            {
                AccessToken = sessionAuth.Token,
                RefreshToken = sessionAuth.RefreshToken
            };
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.RefreshToken, tokenDto);
            if (!(response?.IsSuccess ?? false))
            {
                // If the refresh token request fails, redirect to the login page
                HttpContext.Response.Redirect(Constants.RedirectToLogin);
            }
            sessionAuth = response?.Data;
            HttpContext.Session.Set(Constants.AuthResponseDto, sessionAuth);
        }

        _httpService.SetBearerToken(sessionAuth!.Token ?? "");
        var data = await _httpService.GetAsync<ApiResponse<IList<DynamicMenuItemDto>>?>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.GetByUserId);
        var menuList = data?.Data ?? [];

        List<DynamicMenuItemDto>? _menuList;
        var SessionMenu = HttpContext.Session.Get<IList<DynamicMenuItemDto>>(Constants.Menu);
        if (SessionMenu != null)
        {
            _menuList = (List<DynamicMenuItemDto>?)SessionMenu;
        }
        else
        {
            _menuList = [.. menuList];
            HttpContext.Session.Set(Constants.Menu, menuList);
        }

        return View("_Menu", _menuList);
    }
}
