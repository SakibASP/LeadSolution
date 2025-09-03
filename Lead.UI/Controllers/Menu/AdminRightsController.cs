using Common.Utils.Constant;
using Core.Models.Menu;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Lead.UI.Controllers.Menu;

public class AdminRightsController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private void SetToken() => _httpService.SetBearerToken(UserInfo.AccessToken);
    private string VersionedAdminRights => _apiSettings.Controllers.AdminRights;
    private string VersionedRoles => _apiSettings.Controllers.Roles;

    public async Task<IActionResult> Index()
    {
        SetToken();

        var rolesTask = _httpService.GetAsync<ApiResponse<IList<RoleDto>>>(VersionedRoles, _apiSettings.Endpoints.Roles.GetRoles);
        var menusTask = _httpService.GetAsync<ApiResponse<IList<MenuItem>>>(VersionedAdminRights, _apiSettings.Endpoints.AdminRights.GetAllMenu);

        await Task.WhenAll(rolesTask, menusTask);

        var rolesResponse = rolesTask.Result;
        var menusResponse = menusTask.Result;

        if (rolesResponse?.IsSuccess != true)
            TempData[Constants.Error] = rolesResponse?.Message;

        if (menusResponse?.IsSuccess != true)
            TempData[Constants.Error] = menusResponse?.Message;

        ViewBag.Roles = rolesResponse?.Data?
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
            .ToList() ?? [];

        ViewBag.menusList = menusResponse?.Data ?? [];

        return View();
    }

    public async Task<JsonResult> GetRoleWiseSelectedPages(string roleId)
    {
        SetToken();

        var queryParams = new Dictionary<string, string> { ["roleId"] = roleId };

        var response = await _httpService.GetAsync<ApiResponse<IList<MenuItemViewModel>>>(
            VersionedAdminRights, _apiSettings.Endpoints.AdminRights.GetRoleWiseMenu, queryParams);

        return Json(response?.Data, new JsonSerializerOptions());
    }

    public async Task<JsonResult> UpdateRecords(List<MenuSelectionDto>? model, string roleId)
    {
        SetToken();

        var request = new UpdateAdminRightsRequest
        {
            MenuSelections = model,
            RoleId = roleId
        };

        var response = await _httpService.PostAsync<ApiResponse<string>>(
            VersionedAdminRights, _apiSettings.Endpoints.AdminRights.UpdateRoleWiseMenu, request);

        return Json(response?.Data);
    }
}
