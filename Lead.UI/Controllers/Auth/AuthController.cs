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

namespace Lead.UI.Controllers.Auth;

/// <summary>
/// Md. Sakibur Rahman
/// 19 July 2025
/// </summary>

public class AuthController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
{
    private readonly IHttpService _httpService = httpService;
    private readonly ApiSettings _apiSettings = apiSetting.Value;

    #region Login & register , No bearer needed
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

    [HttpPost]
    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return View(nameof(Login));
    }

    private async Task LoginAsync(LoginDto loginDto)
    {
        var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Login, loginDto);
        if (response is not null && response.IsSuccess)
        {
            HttpContext.Session.Set(Constants.AuthResponseDto, response?.Data);
            ViewBag.AuthResponseDto = response;
        }
        else
        {
            HttpContext.Session.Clear();
        }
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
    #endregion


    public async Task<IActionResult> RoleList()
    {
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
        if (sessionAuth is null) return RedirectToAction(nameof(Login));
        _httpService.SetBearerToken(sessionAuth.Token ?? "");

        var response = await _httpService.GetAsync<ApiResponse<IList<RoleDto>>?>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetRoles);
        if (response is null) return RedirectToAction(nameof(Login));
        ViewBag.AuthResponseDto = sessionAuth;
        return View(response.Data);
    }


    public async Task<IActionResult> AddRole(string roleName)
    {
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
        if (sessionAuth is null) return RedirectToAction(nameof(Login));
        _httpService.SetBearerToken(sessionAuth.Token ?? "");

        var response = await _httpService.PostAsync<ApiResponse<string>>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.AddRole, roleName);
        if (response is null) return RedirectToAction(nameof(Login));

        if (response.IsSuccess)
            TempData[Constants.Success] = response.Message;
        else
            TempData[Constants.Error] = response.Message;

        ViewBag.AuthResponseDto = sessionAuth;
        return RedirectToAction(nameof(RoleList));
    }


    public async Task<IActionResult> EditRole(string roleId)
    {
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
        if (sessionAuth is null) return RedirectToAction(nameof(Login));
        _httpService.SetBearerToken(sessionAuth.Token ?? "");

        var queryParams = new Dictionary<string, string>
        {
            { "roleId", roleId } // or just roleId if it's already string
        };

        var response = await _httpService.GetAsync<ApiResponse<RoleDto>>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetRoleById, queryParams);
        if (response is null) return RedirectToAction(nameof(Login));
        ViewBag.AuthResponseDto = sessionAuth;
        return View(response.Data);
    }


    public async Task<IActionResult> UpdateRole(RoleDto role)
    {
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
        if (sessionAuth is null) return RedirectToAction(nameof(Login));
        _httpService.SetBearerToken(sessionAuth.Token ?? "");

        var response = await _httpService.PostAsync<ApiResponse<string>>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.UpdateRole, role);
        if (response is null) return RedirectToAction(nameof(Login));

        if (response.IsSuccess)
            TempData[Constants.Success] = response.Message;
        else
            TempData[Constants.Error] = response.Message;

        ViewBag.AuthResponseDto = sessionAuth;
        return RedirectToAction(nameof(RoleList));
    }


    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var sessionAuth = HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
        if (sessionAuth is null) return RedirectToAction(nameof(Login));
        _httpService.SetBearerToken(sessionAuth.Token ?? "");

        var response = await _httpService.PostAsync<ApiResponse<string>>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.DeleteRole, roleId);
        if (response is null) return RedirectToAction(nameof(Login));

        if (response.IsSuccess)
            TempData[Constants.Success] = response.Message;
        else
            TempData[Constants.Error] = response.Message;

        ViewBag.AuthResponseDto = sessionAuth;
        return RedirectToAction(nameof(RoleList));
    }
}
