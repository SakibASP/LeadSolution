using Common.Extentions;
using Core.ViewModels.Dto.Auth.Auth;
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
    public IActionResult Index(int? businessId)
    {
        var selectedId = businessId ?? UserBusinessList?.FirstOrDefault()?.Id ?? 0;
        ViewBag.BusinessId = new SelectList(UserBusinessList, "Id", "Name", selectedId);
        return View();
    }

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
