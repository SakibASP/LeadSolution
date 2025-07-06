using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers
{
    public class BaseController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
    {
        protected readonly IHttpService _httpService = httpService;
        protected readonly ApiSettings _apiSettings = apiSetting.Value;
        protected string? AccessToken { get; set; } = string.Empty;
        public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
            // Check if the user is authenticated
            if (sessionAuth?.Token is null)
            {
                //Redirect to the login page
                HttpContext.Response.Redirect(Constants.RedirectToLogin);
                return;
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
                    return;
                }
                sessionAuth = response?.Data;
                HttpContext.Session.Set(Constants.AuthResponseDto, sessionAuth);
            }
            AccessToken = sessionAuth?.Token;
            // Proceed with the action execution
            await next();
        }
    }
}
