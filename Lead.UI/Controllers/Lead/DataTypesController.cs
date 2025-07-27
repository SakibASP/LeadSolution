using Common.Utils.Constant;
using Core.Models.Lead;
using Core.ViewModels.Response;
using Lead.UI.Controllers.Common;
using Lead.UI.Interfaces;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lead.UI.Controllers.Lead;

public class DataTypesController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
{

    private string ApiVersion => _apiSettings.Versions.DataTypes;
    private string ApiController => _apiSettings.ControllerNames.DataTypes;
    private string VersionedController => $"{ApiVersion}/{ApiController}";

    private void SetToken() => _httpService.SetBearerToken(AccessToken);

    public async Task<IActionResult> Index()
    {
        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<IList<DataTypes>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true)
            TempData[Constants.Error] = response?.Message;

        return View(response?.Data ?? []);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DataTypes dataTypes)
    {
        if (!ModelState.IsValid)
            return View(dataTypes);

        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Add, dataTypes);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        SetToken();
        var response = await _httpService.GetAsync<ApiResponse<DataTypes>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetById,
            new() { ["id"] = id.ToString()! });

        return response?.Data is null ? NotFound() : View(response.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DataTypes dataTypes)
    {
        if (id != dataTypes.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(dataTypes);

        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Update, dataTypes);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int id)
    {
        SetToken();
        var response = await _httpService.PostAsync<ApiResponse<DataTypes>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.Remove, id);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

}
