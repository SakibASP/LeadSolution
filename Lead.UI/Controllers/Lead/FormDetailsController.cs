using Common.Utils.Constant;
using Core.Models.Lead;
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
    private string ApiVersion => _apiSettings.Versions.FormDetails;
    private string ApiController => _apiSettings.ControllerNames.FormDetails;
    private string VersionedController => $"{ApiVersion}/{ApiController}";

    private void SetToken() => _httpService.SetBearerToken(AccessToken);

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
        var viewBagResponse = await _httpService.GetAsync<ApiResponse<IList<DataTypes>>>(
            $"{_apiSettings.Versions.DataTypes}/{_apiSettings.ControllerNames.DataTypes}", _apiSettings.Endpoints.CommonEndPoints.GetAll);

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
        var postTask = _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController,
            _apiSettings.Endpoints.CommonEndPoints.Add,
            formDetails);

        var getTask = _httpService.GetAsync<ApiResponse<IList<DataTypes>>>(
            $"{_apiSettings.Versions.DataTypes}/{_apiSettings.ControllerNames.DataTypes}",
            _apiSettings.Endpoints.CommonEndPoints.GetAll);

        await Task.WhenAll(postTask, getTask);

        var response = await postTask;
        var viewBagResponse = await getTask;

        ViewBag.TypeId = new SelectList(viewBagResponse?.Data, "Id", "Name", formDetails.TypeId);
        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return View(formDetails);
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        SetToken();
        var getDetailTask = _httpService.GetAsync<ApiResponse<FormDetails>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.GetById,
            new() { ["id"] = id.ToString()! });
        var getTypeTask = _httpService.GetAsync<ApiResponse<IList<DataTypes>>>(
            $"{_apiSettings.Versions.DataTypes}/{_apiSettings.ControllerNames.DataTypes}",
            _apiSettings.Endpoints.CommonEndPoints.GetAll);

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
        var postTask = _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.Update, formdetails);
        var getTask = _httpService.GetAsync<ApiResponse<IList<DataTypes>>>(
            $"{_apiSettings.Versions.DataTypes}/{_apiSettings.ControllerNames.DataTypes}",
            _apiSettings.Endpoints.CommonEndPoints.GetAll);

        await Task.WhenAll(postTask, getTask);

        var response = await postTask;
        var viewBagResponse = await getTask;

        ViewBag.TypeId = new SelectList(viewBagResponse?.Data, "Id", "Name", response?.Data?.TypeId);
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
