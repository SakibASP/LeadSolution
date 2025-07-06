using LeadUI.Interfaces;
using LeadUI.Models;
using LeadUI.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Utils.Helper;
using ViewModels.Auth;

namespace LeadUI.Controllers
{
    public class HomeController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
    {
        public IActionResult Index()
        {
            var name = "Lead";
            ViewBag.Name = name;
            return View();
        }

        public IActionResult Privacy()
        {
            var _response = HttpContext.Session.Get<AuthResponseDto>("AuthResponseDto");
            return View(_response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
