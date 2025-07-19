using Common.Utils.Constant;
using Core.Models.Menu;
using Core.ViewModels.Dto.Auth;
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
    public async Task<IActionResult> Index()
    {
        _httpService.SetBearerToken(AccessToken);
        var rolesResponse = await _httpService.GetAsync<ApiResponse<IList<RoleDto>>>(_apiSettings.Versions.Auth, _apiSettings.Endpoints.Auth.GetRoles);
        var menusResponse = await _httpService.GetAsync<ApiResponse<IList<MenuItem>>>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.GetAllMenu);
        if (rolesResponse?.IsSuccess is not true) TempData[Constants.Error] = rolesResponse?.Message;
        if (menusResponse?.IsSuccess is not true) TempData[Constants.Error] = menusResponse?.Message;
        ViewBag.Roles = rolesResponse?.Data?
                .OrderBy(r => r.Name)
                .ToList()
                .Select(rr => new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name })
                ?? [];
        ViewBag.menusList = menusResponse?.Data ?? [];
        return View();
    }

    public async Task<JsonResult> GetRoleWiseSelectedPages(string roleId)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "roleId", roleId } // or just roleId if it's already string
        };
        _httpService.SetBearerToken(AccessToken);
        var response = await _httpService.GetAsync<ApiResponse<IList<MenuItemViewModel>>>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.GetRoleWiseMenu, queryParams);
        return Json(response?.Data, new JsonSerializerOptions());
    }

    public async Task<JsonResult> UpdateRecords(List<MenuSelectionDto>? model, string roleId)
    {
        UpdateAdminRightsRequest request = new() { MenuSelections = model, RoleId = roleId };
        _httpService.SetBearerToken(AccessToken);
        var response = await _httpService.PostAsync<ApiResponse<string>>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.UpdateRoleWiseMenu, request);
        return Json(response?.Data);
    }
}
