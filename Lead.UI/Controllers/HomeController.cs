using Common.Utils.Helper;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Response;
using Lead.UI.Interfaces;
using Lead.UI.Models;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lead.UI.Controllers
{
    public class HomeController(IHttpService httpService, IOptions<ApiSettings> apiSetting) : BaseController(httpService, apiSetting)
    {
        public IActionResult Index()
        {
            var name = "Lead";
            ViewBag.Name = name;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var queryParams = new Dictionary<string, string>
                                {
                                    { "userId", "abc123" }
                                };
            _httpService.SetBearerToken(AccessToken);
            var data = await _httpService.GetAsync<ApiResponse<IList<DynamicMenuItemDto>>?>(_apiSettings.Versions.Menu, _apiSettings.Endpoints.Menu.GetByUserId, queryParams);
            var response = HttpContext.Session.Get<AuthResponseDto>("AuthResponseDto");
            return View(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
