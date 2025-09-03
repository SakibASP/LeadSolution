using Common.Utils.Constant;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Lead.UI.Controllers.Auth;

public class RolesController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string VersionedController => _apiSettings.Controllers.Roles;
    private void SetToken() => _httpService.SetBearerToken(UserInfo.AccessToken);
    public async Task<IActionResult> RoleList()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<RoleDto>>>(
            VersionedController, _apiSettings.Endpoints.Roles.GetRoles);

        return response == null ? RedirectToAction(nameof(Login)) : View(response.Data);
    }

    public async Task<IActionResult> AddRole(string roleName)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<string>>(
            VersionedController, _apiSettings.Endpoints.Roles.AddRole, roleName);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }

    public async Task<IActionResult> EditRole(string roleId)
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<RoleDto>>(
            VersionedController, _apiSettings.Endpoints.Roles.GetRoleById,
            new Dictionary<string, string> { ["roleId"] = roleId });

        return response == null ? RedirectToAction(nameof(Login)) : View(response.Data);
    }

    public async Task<IActionResult> UpdateRole(RoleDto role)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<string>>(
            VersionedController, _apiSettings.Endpoints.Roles.UpdateRole, role);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }

    public async Task<IActionResult> DeleteRole(string roleId)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<string>>(
            VersionedController, _apiSettings.Endpoints.Roles.DeleteRole, roleId);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(RoleList));
    }
}
