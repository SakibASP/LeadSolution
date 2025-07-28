using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Create()
    {
        SetToken();
        var viewBagResponse = await _httpService.GetAsync<ApiResponse<IList<AspNetServiceTypes>>>(
            VersionedController, _apiSettings.Endpoints.BusinessInfo.GetServiceType);

        if (viewBagResponse?.IsSuccess is not true) TempData[Constants.Error] = viewBagResponse?.Message;

        ViewBag.ServiceId = new SelectList(viewBagResponse?.Data, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AspNetBusinessInfo businessInfo)
    {
        if (!ModelState.IsValid)
            return View(businessInfo);

        SetToken();
        var postTask = _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Add, businessInfo);

        var getTask = _httpService.GetAsync<ApiResponse<IList<AspNetServiceTypes>>>(
            VersionedController, _apiSettings.Endpoints.BusinessInfo.GetServiceType);

        await Task.WhenAll(postTask, getTask);

        var response = await postTask;
        var viewBagResponse = await getTask;

        ViewBag.ServiceId = new SelectList(viewBagResponse?.Data, "Id", "Name", businessInfo.ServiceId);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        SetToken();

        var getDetailTask = _httpService.GetAsync<ApiResponse<AspNetBusinessInfo>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetById,
            new() { ["id"] = id.ToString()! });

        var getTypeTask = _httpService.GetAsync<ApiResponse<IList<AspNetServiceTypes>>>(
            VersionedController, _apiSettings.Endpoints.BusinessInfo.GetServiceType);

        await Task.WhenAll(getDetailTask, getTypeTask);

        var response = await getDetailTask;
        var viewBagResponse = await getTypeTask;

        if (viewBagResponse?.IsSuccess is not true) TempData[Constants.Error] = viewBagResponse?.Message;

        ViewBag.ServiceId = new SelectList(viewBagResponse?.Data, "Id", "Name", response?.Data?.ServiceId);
        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
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
        var postTask = _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Update, businessInfo);

        var getTask = _httpService.GetAsync<ApiResponse<IList<AspNetServiceTypes>>>(
            VersionedController, _apiSettings.Endpoints.BusinessInfo.GetServiceType);

        await Task.WhenAll(postTask, getTask);

        var response = await postTask;
        var viewBagResponse = await getTask;

        ViewBag.ServiceId = new SelectList(viewBagResponse?.Data, "Id", "Name", businessInfo.ServiceId);
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

