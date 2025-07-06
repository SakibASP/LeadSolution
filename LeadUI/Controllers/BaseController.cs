using LeadUI.Interfaces;
using LeadUI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Utils.Helper;
using ViewModels.Auth;

namespace LeadUI.Controllers
{
    public class BaseController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
    {
        private readonly IHttpService _httpService = httpService;
        private readonly ApiSettings _apiSettings = apiSetting.Value;
        protected string? AccessToken { get; set; } = string.Empty;
        public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            const string sessionKey = "AuthResponseDto";

            var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(sessionKey);
            // Check if the user is authenticated
            if (sessionAuth?.Token is null || sessionAuth?.Expiration <= TimeHelper.GetCurrentBangladeshTime())
            {
                var loginData = new
                {
                    Email = "sakibur.rahman.cse@gmail.com",
                    Password = "Sakib@123"
                };

                sessionAuth = await _httpService.PostAsync<AuthResponseDto?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Login, loginData);
                if (sessionAuth?.Token != null) HttpContext.Session.Set(sessionKey, sessionAuth);
            }
            AccessToken = sessionAuth?.Token;
            // Proceed with the action execution
            await next();
        }
    }
}
