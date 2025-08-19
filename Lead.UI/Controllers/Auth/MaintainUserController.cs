using Azure.Core;
using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Request.Auth;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Lead.UI.Controllers.Auth;

public class MaintainUserController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string VersionedController => _apiSettings.Controllers.MaintainUser;
    private void SetToken() => _httpService.SetBearerToken(AccessToken);

    public async Task<IActionResult> UserList()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<UserRolesDto>>>(
            VersionedController, _apiSettings.Endpoints.MaintainUser.GetUsers);

        return View(response?.Data ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<ApplicationUser>>(
            VersionedController, _apiSettings.Endpoints.MaintainUser.GetUserProfile);

        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction("Index", "Home");
        }

        ApplicationUser user = response.Data!;
        // Fetch current user details from DB
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            NID = user.NID,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        SetToken();
        if (!ModelState.IsValid)
        {
            TempData[Constants.Error] = "Model in not valid";
            return View(nameof(UserProfile), model);
        }

        var response = await _httpService.PostAsync<ApiResponse<string>>(
             VersionedController, _apiSettings.Endpoints.MaintainUser.UpdateProfile, model);
        if (response?.IsSuccess is true) TempData["UserSuccess"] = response?.Message;
        else TempData[Constants.Error] = response?.Message;
        return View(nameof(UserProfile),model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        SetToken();
        if (!ModelState.IsValid)
        {
            TempData[Constants.Error] = "Model in not valid";
            return View(nameof(UserProfile), model);
        }

        var response = await _httpService.PostAsync<ApiResponse<string>>(
             VersionedController, _apiSettings.Endpoints.MaintainUser.ChangePassword, model);
        if (response?.IsSuccess is true) TempData["UserSuccess"] = response?.Message;
        else TempData[Constants.Error] = response?.Message;
        return View(nameof(UserProfile), model);
    }

    #region - user role management -
    [HttpGet]
    public async Task<IActionResult> ManageUser(string userId)
    {
        SetToken();
        ViewBag.userId = userId;

        var response = await _httpService.GetAsync<ApiResponse<IList<ManageUserRoleDto>>>(
             VersionedController, _apiSettings.Endpoints.MaintainUser.GetUserRoles,
            new Dictionary<string, string> { ["userId"] = userId });

        return View(response?.Data ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> ManageUser(IList<ManageUserRoleDto> model, string userId)
    {
        SetToken();
        var request = new UpdateUserRoleRequest
        {
            UserId = userId,
            ManageUsers = model
        };

        var response = await _httpService.PostAsync<ApiResponse<string>>(
             VersionedController, _apiSettings.Endpoints.MaintainUser.AssignRole, request);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Data;
        return RedirectToAction(nameof(UserList));
    }
    #endregion
}
