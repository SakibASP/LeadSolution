using Common.Utils.Constant;
using Common.Utils.Enums;
using Core.Models.Auth;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Dto.Lead;
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
    private string VersionedController => $"{_apiSettings.Controllers.BusinessInfo}";
    private void SetToken() => _httpService.SetBearerToken(UserInfo.AccessToken);

    private readonly Dictionary<string, string> GetParam = [];

    public async Task<IActionResult> Index()
    {
        SetToken();

        var response = await _httpService.GetAsync<ApiResponse<IList<AspNetBusinessInfo>>>(
            VersionedController, _apiSettings.Endpoints.CommonEndPoints.GetAll);

        if (response?.IsSuccess != true) TempData[Constants.Error] = response?.Message;

        return View(response?.Data ?? []);
    }

    public async Task<IActionResult> Create()
    {
        SetToken();
        (bool flowControl, IActionResult value) = await PrepareCreateBusinessInfoAsync();
        if (!flowControl) return value;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AspNetBusinessInfoDto businessInfo)
    {
        SetToken();
        if (!ModelState.IsValid)
        {
            (bool flowControl, IActionResult value) = await PrepareCreateBusinessInfoAsync();
            if (!flowControl) return value;
            return View(businessInfo);
        }

        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController,
            _apiSettings.Endpoints.CommonEndPoints.Add,
            businessInfo);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        SetToken();

        if (id is null)
            return NotFound();

        (bool flowControl, IActionResult value) = await PrepareEditBusinessInfoAsync(id);
        if (!flowControl) return value;
        if (flowControl && value is not null) return value;
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AspNetBusinessInfo businessInfo)
    {
        SetToken();

        if (id != businessInfo.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            (bool flowControl, IActionResult value) = await PrepareEditBusinessInfoAsync(id);
            if (!flowControl) return value;
            if (flowControl && value is not null) return value;
            return View(businessInfo);
        }

        //saving data
        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.Update, businessInfo);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int id)
    {
        SetToken();

        var response = await _httpService.PostAsync<ApiResponse<dynamic>>(
            VersionedController, 
            _apiSettings.Endpoints.CommonEndPoints.Remove, id);

        TempData[response?.IsSuccess == true ? Constants.Success : Constants.Error] = response?.Message;
        return RedirectToAction(nameof(Index));
    }

    #region - private methods -
    private async Task<(bool flowControl, IActionResult value)> PrepareCreateBusinessInfoAsync()
    {
        GetParam.Clear();
        GetParam.Add("id", ((int)DropdownEnum.ServiceTypes).ToString());
        var serviceDropdownTask = _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
            UtilityVersion, _apiSettings.Endpoints.Utility.GetDropdown, GetParam);

        GetParam.Clear();
        GetParam.Add("id", ((int)DropdownEnum.Users).ToString());
        var userDropdownTask = _httpService.GetAsync<ApiResponse<IList<UserDropdownDto>>>(
             UtilityVersion, _apiSettings.Endpoints.Utility.GetUserDropdown, GetParam);

        await Task.WhenAll(serviceDropdownTask, userDropdownTask);

        var serviceResponse = await serviceDropdownTask;
        if (serviceResponse?.IsSuccess is not true)
        {
            TempData[Constants.Error] = serviceResponse?.Message;
            return (flowControl: false, value: RedirectToAction(nameof(Index)));
        }

        var userResponse = await userDropdownTask;
        if (userResponse?.IsSuccess is not true)
        {
            TempData[Constants.Error] = userResponse?.Message;
            return (flowControl: false, value: RedirectToAction(nameof(Index)));
        }

        ViewBag.Admin = UserInfo.UserRoles?.Contains(Constants.Admin) is true || UserInfo.UserRoles?.Contains(Constants.SuperAdmin) is true;
        ViewBag.ServiceId = new SelectList(serviceResponse?.Data, "Id", "Name");
        ViewBag.UserId = new SelectList(userResponse?.Data, "Id", "Name");
        return (flowControl: true, value: null!);
    }

    private async Task<(bool flowControl, IActionResult value)> PrepareEditBusinessInfoAsync(int? id)
    {
        // getting the row to edit
        GetParam.Clear();
        GetParam.Add("id", id.ToString()!);
        var businessInfoTask = _httpService.GetAsync<ApiResponse<AspNetBusinessInfo>>(
            VersionedController,
            _apiSettings.Endpoints.CommonEndPoints.GetById,
            GetParam);

        // getting service type dropdown 
        GetParam.Clear();
        GetParam.Add("Id", ((int)DropdownEnum.ServiceTypes).ToString());
        var serviceDropdownTask = _httpService.GetAsync<ApiResponse<IList<DropdownDto>>>(
            UtilityVersion,
            _apiSettings.Endpoints.Utility.GetDropdown,
            GetParam);

        // getting user dropdown 
        GetParam.Clear();
        GetParam.Add("Id", ((int)DropdownEnum.BusinessWiseUsers).ToString());
        GetParam.Add("Param1", id.ToString()!);
        var userDropdownTask = _httpService.GetAsync<ApiResponse<IList<UserDropdownDto>>>(
            UtilityVersion,
            _apiSettings.Endpoints.Utility.GetUserDropdown,
            GetParam);

        await Task.WhenAll(businessInfoTask, serviceDropdownTask, userDropdownTask);

        var response = await businessInfoTask;
        var serviceResponse = await serviceDropdownTask;
        var userResponse = await userDropdownTask;

        if (serviceResponse?.IsSuccess is not true)
        {
            TempData[Constants.Error] = serviceResponse?.Message;
            return (flowControl: false, value: RedirectToAction(nameof(Index)));
        }
        if (userResponse?.IsSuccess is not true)
        {
            TempData[Constants.Error] = userResponse?.Message;
            return (flowControl: false, value: RedirectToAction(nameof(Index)));
        }

        ViewBag.ServiceId = new SelectList(serviceResponse?.Data, "Id", "Name", response?.Data?.ServiceId);
        ViewBag.SelectedUserName = userResponse?.Data?.Select(x=>x.Name).FirstOrDefault();
        ViewBag.Admin = UserInfo.UserRoles?.Contains(Constants.Admin) is true || UserInfo.UserRoles?.Contains(Constants.SuperAdmin) is true;

        if (response?.IsSuccess is not true)
            return (flowControl: false, value: NotFound());

        return (flowControl: true, value: View(response?.Data));
    }
    #endregion
}

