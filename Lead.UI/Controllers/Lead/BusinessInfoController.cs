using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Lead;

public class BusinessInfoController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string ApiVersion => _apiSettings.Versions.BusinessInfo;
    private string ApiController => _apiSettings.ControllerNames.BusinessInfo;
    private string VersionedController => $"{ApiVersion}/{ApiController}";

    private void SetToken() => _httpService.SetBearerToken(AccessToken);

    public async Task<IActionResult> Index()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<AspNetBusinessInfo>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true)
            TempData[Constants.Error] = response?.Message;

        return View(response?.Data ?? []);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AspNetBusinessInfo businessInfo)
    {
        if (!ModelState.IsValid)
            return View(businessInfo);

        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Add, businessInfo);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<AspNetBusinessInfo>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetById,
            new() { ["id"] = id.ToString()! });

        return response?.Data is null ? NotFound() : View(response.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AspNetBusinessInfo businessInfo)
    {
        if (id != businessInfo.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(businessInfo);

        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Update, businessInfo);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int id)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Remove, id);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }
}

