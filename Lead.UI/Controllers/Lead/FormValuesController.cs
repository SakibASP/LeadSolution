using Common.Utils.Constant;
using Common.Utils.Enums;
using Core.Models.Lead;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Lead;

public class FormValuesController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string UtilityVersion => $"{_apiSettings.Controllers.Utility}";
    private string VersionedController => _apiSettings.Controllers.FormValues;
    private void SetToken() => _httpService.SetBearerToken(AccessToken);
    private readonly Dictionary<string, string> GetParam = [];
    public async Task<IActionResult> Index()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<FormValues>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true)
            TempData[Constants.Error] = response?.Message;

        ViewBag.Headers = response?.Data?.Select(x => x.FormDetails!.Name).ToList();
        // 1. Get all FormDetail names
        //ViewBag.Headers = await _context.FormDetails
        //    .OrderBy(f => f.Id)
        //    .Select(f => f.Name)
        //    .ToListAsync();
        return View(response?.Data ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> DynamicForm(int? businessId)
    {
        SetToken();

        GetParam.Clear();
        GetParam.Add("Id", ((int)DropdownEnum.UserWiseBusinesses).ToString());
        var viewBagResponse = await _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
             UtilityVersion, _apiSettings.Endpoints.Utility.GetDropdown, GetParam);

        if (viewBagResponse?.IsSuccess is not true) TempData[Constants.Error] = viewBagResponse?.Message;

        var selectedId = businessId ?? viewBagResponse?.Data?.FirstOrDefault()?.Id ?? 0;
        GetParam.Clear();
        GetParam.Add("businessId", selectedId.ToString());
        var response = await _httpService.GetAsync<ApiResponse<DynamicFormViewModel>>(
            VersionedController,
            _apiSettings.Endpoints.FormValues.GetDynamicForm, GetParam);

        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction("Index","Home");
        }

        ViewBag.BusinessId = new SelectList(viewBagResponse?.Data, "Id", "Name", selectedId);
        return View(response?.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DynamicForm(DynamicFormViewModel formDetails)
    {
        if (!ModelState.IsValid)
            return View(formDetails);

        //SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.Add, formDetails);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFormSettings([FromBody] UpdateFormSettingsRequest request)
    {

        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController,
            _apiSettings.Endpoints.FormValues.UpdateFormSettings, request);

        return Json(response);
    }

}
