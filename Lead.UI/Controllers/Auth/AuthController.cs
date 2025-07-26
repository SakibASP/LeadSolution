using Common.Extentions;
using Common.Utils.Constant;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Request.Auth;
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

    private AuthResponseDto? SessionAuth => HttpContext.Session.Get<AuthResponseDto>(Constants.AuthResponseDto);
    private bool EnsureSessionToken()
    {
        if (SessionAuth is null) return false;
        _httpService.SetBearerToken(SessionAuth.Token ?? "");
        ViewBag.AuthResponseDto = SessionAuth;
        return true;
    }

    #region Login & Register
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
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Login, loginDto);

        if (response?.IsSuccess == true)
        {
            HttpContext.Session.Set(Constants.AuthResponseDto, response.Data);
            ViewBag.AuthResponseDto = response.Data;
        }
        else
        {
            HttpContext.Session.Clear();
        }
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var response = await _httpService.PostAsync<ApiResponse<AuthResponseDto>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.Register, registerDto);

        if (response?.IsSuccess == true)
            await LoginAsync(new() { Email = registerDto.Email, Password = registerDto.Password });

        return RedirectToAction("Index", "Home");
    }
    #endregion

    #region Role Management
    public async Task<IActionResult> RoleList()
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.GetAsync<ApiResponse<IList<RoleDto>>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetRoles);

        return response == null ? RedirectToAction(nameof(Login)) : View(response.Data);
    }

    public async Task<IActionResult> AddRole(string roleName)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.PostAsync<ApiResponse<string>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.AddRole, roleName);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }

    public async Task<IActionResult> EditRole(string roleId)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.GetAsync<ApiResponse<RoleDto>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetRoleById,
            new Dictionary<string, string> { ["roleId"] = roleId });

        return response == null ? RedirectToAction(nameof(Login)) : View(response.Data);
    }

    public async Task<IActionResult> UpdateRole(RoleDto role)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.PostAsync<ApiResponse<string>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.UpdateRole, role);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }

    public async Task<IActionResult> DeleteRole(string roleId)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.PostAsync<ApiResponse<string>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.DeleteRole, roleId);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }
    #endregion

    #region User Management
    public async Task<IActionResult> UserList()
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var response = await _httpService.GetAsync<ApiResponse<IList<UserRolesDto>>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetUsers);

        return View(response?.Data ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> ManageUser(string userId)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        ViewBag.userId = userId;

        var response = await _httpService.GetAsync<ApiResponse<IList<ManageUserRoleDto>>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetUserRoles,
            new Dictionary<string, string> { ["userId"] = userId });

        return View(response?.Data ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> ManageUser(IList<ManageUserRoleDto> model, string userId)
    {
        if (!EnsureSessionToken()) return RedirectToAction(nameof(Login));

        var request = new UpdateUserRoleRequest
        {
            UserId = userId,
            ManageUsers = model
        };

        var response = await _httpService.PostAsync<ApiResponse<string>>(
            _apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.AssignRole, request);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Data;
        return RedirectToAction(nameof(UserList));
    }
    #endregion
}
