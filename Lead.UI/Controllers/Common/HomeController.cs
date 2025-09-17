using Common.Utils.Extentions;
using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.Models.Common;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Models;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Lead.UI.Controllers.Common;

public class HomeController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private void SetToken() => _httpService.SetBearerToken(UserInfo.AccessToken);
    private readonly Dictionary<string, string> GetParam = [];

    public IActionResult Index(int? businessId)
    {
        var selectedId = businessId ?? UserInfo.UserBusinessList?.FirstOrDefault()?.Id ?? 0;
        ViewBag.BusinessId = new SelectList(UserInfo.UserBusinessList, "Id", "Name", selectedId);
        return View();
    }

    #region - logs -
    public async Task<IActionResult> SystemLogs()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<Logs>>>(
            UtilityVersion, _apiSettings.Endpoints.Utility.GetSystemLogs);
        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response?.Data);
    }

    public async Task<IActionResult> SystemLogDetail(int id)
    {
        SetToken();
        GetParam.Clear();
        GetParam.Add("id", id.ToString());
        var response = await _httpService.GetAsync<ApiResponse<Logs>>(
            UtilityVersion, 
            _apiSettings.Endpoints.Utility.GetSystemLogById,
            GetParam);
        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction(nameof(SystemLogs));
        }

        ViewBag.Properties = ParseHelper.ParseXmlToDictionary(response?.Data?.Properties);
        return View(response?.Data);
    }

    public async Task<IActionResult> ApiLogs()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<RequestLogs>>>(
            UtilityVersion, _apiSettings.Endpoints.Utility.GetApiLogs);
        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(response?.Data);
    }

    public async Task<IActionResult> ApiLogDetail(int id)
    {
        SetToken();
        GetParam.Clear();
        GetParam.Add("id", id.ToString());
        var response = await _httpService.GetAsync<ApiResponse<RequestLogs>>(
            UtilityVersion, 
            _apiSettings.Endpoints.Utility.GetApiLogById,
            GetParam);
        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction(nameof(SystemLogs));
        }

        return View(response?.Data);
    }

    #endregion

    public IActionResult Privacy()
    {
        var response = HttpContext.Session.Get<AuthResponseDto>("AuthResponseDto");
        return View(response);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
