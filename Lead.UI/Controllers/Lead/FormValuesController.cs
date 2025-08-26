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
using System.Text.Json;

namespace Lead.UI.Controllers.Lead;

public class FormValuesController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{
    private string Version => _apiSettings.Controllers.FormValues;
    private FormValuesEndpoints Endpoints => _apiSettings.Endpoints.FormValues;

    private void SetToken() => _httpService.SetBearerToken(AccessToken);
    private readonly Dictionary<string, string> GetParam = [];

    public async Task<IActionResult> Index(int? businessId)
    {
        SetToken();

        var selectedId = businessId ?? UserBusinessList?.FirstOrDefault()?.Id ?? 0;

        GetParam.Clear();
        GetParam.Add("businessId", selectedId.ToString());
        var response = await _httpService.GetAsync<ApiResponse<dynamic>>(
            Version, 
            CommonEndpoints.GetAll, 
            GetParam);

        if (response?.IsSuccess != true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction("Index", "Home");
        }

        var rows = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(response?.Data);
        ViewBag.DynamicRows = rows;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DynamicForm(int? businessId)
    {
        SetToken();

        var selectedId = businessId ?? UserBusinessList?.FirstOrDefault()?.Id ?? 0;

        GetParam.Clear();
        GetParam.Add("businessId", selectedId.ToString());
        var response = await _httpService.GetAsync<ApiResponse<DynamicFormViewModel>>(
            Version,
            Endpoints.GetDynamicForm, 
            GetParam);

        if (response?.IsSuccess is not true)
        {
            TempData[Constants.Error] = response?.Message;
            return RedirectToAction("Index","Home");
        }

        ViewBag.ApiKey = await GetActiveApiKey(selectedId);
        ViewBag.BusinessId = new SelectList(UserBusinessList, "Id", "Name", selectedId);
        return View(response?.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DynamicForm(DynamicFormViewModel formDetails)
    {
        if (!ModelState.IsValid)
            return View(formDetails);

        SetToken();
        var headers = new Dictionary<string, string>
            {
                { "X-Api-Key", await GetActiveApiKey(formDetails.BusinessId) },
                { "X-Business-Id", formDetails.BusinessId.ToString() }
            };

        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            Version, 
            CommonEndpoints.Add, 
            formDetails,
            headers);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFormSettings([FromBody] UpdateFormSettingsRequest request)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            Version,
            Endpoints.UpdateFormSettings, 
            request);

        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateNewApiKey([FromBody] int businessId)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<string>>(
            _apiSettings.Controllers.Auth,
            _apiSettings.Endpoints.Auth.GenerateApiKey, businessId);

        return Json(response);
    }

}
