using Common.Utils.Constant;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Lead;

public class FormValuesController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{

    private string ApiVersion => _apiSettings.Versions.FormValues;
    private string ApiController => _apiSettings.ControllerNames.FormValues;
    private string VersionedController => $"{ApiVersion}/{ApiController}";

    public async Task<IActionResult> Index()
    {
        //SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<FormValues>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true)
            TempData[Constants.Error] = response?.Message;

        return View(response?.Data ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> DynamicForm()
    {
        var response = await _httpService.GetAsync<ApiResponse<DynamicFormViewModel>>(
            VersionedController,
            _apiSettings.Endpoints.FormValues.GetDynamicForm);

        if (response?.IsSuccess is not true) TempData[Constants.Error] = response?.Message;
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
}
