using Common.Extentions;
using Common.Utils.Constant;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Auth;

/// <summary>
/// Md. Sakibur Rahman
/// 19 July 2025
/// </summary>

public class AuthController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
{
    private readonly IHttpService _httpService = httpService;
    private readonly ApiSettings _apiSettings = apiSetting.Value;
    private string VersionedController => _apiSettings.Controllers.Auth;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        await LoginAsync(loginDto);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return View(nameof(Login));
    }

    private async Task LoginAsync(LoginDto loginDto)
    {
        var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>>(
            VersionedController, _apiSettings.Endpoints.Auth.Login, loginDto);

        if (response?.IsSuccess == true)
        {
            HttpContext.Session.Set(Constants.AuthResponseDto, response.Data);
            ViewBag.AuthResponseDto = response.Data;
        }
        else
        {
            HttpContext.Session.Clear();
            TempData[Constants.Error] = response?.Message;
        }
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>>(
            VersionedController, _apiSettings.Endpoints.Auth.Register, registerDto);

        if (response?.IsSuccess == true)
            await LoginAsync(new() { Email = registerDto.Email, Password = registerDto.Password });

        return RedirectToAction("Index", "Home");
    }
}
