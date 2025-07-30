using Common.Utils.Constant;
using Common.Utils.Enums;
using Core.Models.Lead;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Lead;

public class FormDetailsController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string UtilityVersion => $"{_apiSettings.Controllers.Utility}";
    private string VersionedController => $"{_apiSettings.Controllers.FormDetails}";

    private void SetToken() => _httpService.SetBearerToken(AccessToken);

    private readonly Dictionary<string, string> GetParam = [];

    public async Task<IActionResult> Index()
    {
        SetToken();

        var response = await _httpService.GetAsync<ApiResponse<IList<FormDetails>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true)
            TempData[Constants.Error] = response?.Message;

        return View(response?.Data ?? []);
    }

    public async Task<IActionResult> Create()
    {
        SetToken();

        GetParam.Clear();
        GetParam.Add("Id", ((int)DropdownEnum.DataTypes).ToString());
        var viewBagResponse = await _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
             UtilityVersion, _apiSettings.Endpoints.Utility.GetDropdown, GetParam);

        if (viewBagResponse?.IsSuccess is not true) TempData[Constants.Error] = viewBagResponse?.Message;

        ViewBag.TypeId = new SelectList(viewBagResponse?.Data, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FormDetails formDetails)
    {
        if (!ModelState.IsValid)
            return View(formDetails);

        SetToken();

        // Start both tasks
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController,
            _apiSettings.Endpoints.CommonEndPoints.Add,
            formDetails);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        SetToken();

        GetParam.Clear();
        GetParam.Add("Id", id.ToString()!);
        var getDetailTask = _httpService.GetAsync<ApiResponse<FormDetails>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.GetById, GetParam);

        GetParam.Clear();
        GetParam.Add("Id", ((int)DropdownEnum.DataTypes).ToString());
        var getTypeTask = _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
             UtilityVersion, _apiSettings.Endpoints.Utility.GetDropdown, GetParam);

        await Task.WhenAll(getDetailTask, getTypeTask);

        var response = await getDetailTask;
        var viewBagResponse = await getTypeTask;

        if (viewBagResponse?.IsSuccess is not true) TempData[Constants.Error] = viewBagResponse?.Message;

        ViewBag.TypeId = new SelectList(viewBagResponse?.Data, "Id", "Name", response?.Data?.TypeId);
        return response?.Data is null ? NotFound() : View(response.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FormDetails formdetails)
    {
        if (id != formdetails.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(formdetails);

        SetToken();

        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.Update, formdetails);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int id)
    {
        SetToken();

        var response = await _httpService.PostAsync<ApiResponse<FormDetails>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Remove, id);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

}
