using LeadUI.Interfaces;
using LeadUI.Models;
using LeadUI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Utils.Constant;
using ViewModels.Auth;

namespace LeadUI.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IHttpService httpService, IOptions<ApiSettings> apiSetting) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IHttpService _httpService = httpService;
        private readonly ApiSettings _apiSettings = apiSetting.Value;
        public IActionResult Index()
        {
            var name = "Lead";
            ViewBag.Name = name;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var loginData = new
            {
                Email = "sakibur.rahman.cse@gmail.com",
                Password = "Sakib@123"
            };

            var _url = _apiSettings.Versions.Auth + _apiSettings.Endpoints.Auth.Login;
            var response = await _httpService.PostAsync<AuthResponseDto>(_url, loginData);
            return View(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
