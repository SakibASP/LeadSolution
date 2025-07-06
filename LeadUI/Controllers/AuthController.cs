using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers
{
    public class AuthController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
    {
        private readonly IHttpService _httpService = httpService;
        private readonly ApiSettings _apiSettings = apiSetting.Value;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            await LoginAsync(loginDto);
            return RedirectToAction("Index", "Home");
        }

        private async Task LoginAsync(LoginDto loginDto)
        {
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Login, loginDto);
            if (response is not null && response.IsSuccess) HttpContext.Session.Set(Constants.AuthResponseDto, response?.Data);
            else HttpContext.Session.Clear();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var response = await _httpService.GetAsync<ApiResponse<IList<AspNetServiceTypes>>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.ServiceType);
            var responseList = response?.Data ?? [];
            ViewBag.ServiceTypes = new SelectList(responseList, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Register, registerDto);
            if (response is not null && response.IsSuccess) await LoginAsync(new() { Email = registerDto.Email,Password = registerDto.Password });
            return RedirectToAction("Index", "Home");
        }
    }
}
