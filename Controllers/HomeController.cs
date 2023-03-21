using CalorieTrackerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CalorieTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISession session;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            session = httpContextAccessor.HttpContext.Session;
        }   

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Dashboard()
        {
            if (session.GetInt32("ID") != null)
            {
                DashboardModel model = new DashboardModel();
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://localhost:7230/api/Home/"+session.GetInt32("ID"));
                if (response != null)
                {
                    if(response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        model = JsonConvert.DeserializeObject<DashboardModel>(content);
                        return View(model);
                    }
                }
                TempData["error"] = "An error occured!";
                return RedirectToAction("Index");
            }
            TempData["warning"] = "Please login to access the dashboard!";
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}