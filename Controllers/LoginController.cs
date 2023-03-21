using CalorieTrackerWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CalorieTrackerAPI.Models;
using Newtonsoft.Json;

namespace CalorieTrackerWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly ISession session;

        public LoginController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User u)
        {
            if(u.Email != null && u.Password != null)
            {
                LoginModel lm = new LoginModel(u.Email, u.Password);
                HttpClient httpClient = new HttpClient();
                StringContent s = new StringContent(JsonConvert.SerializeObject(lm),System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://localhost:7230/api/Login", s);
                if(response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        User curr = new User();
                        string content = await response.Content.ReadAsStringAsync();
                        curr = JsonConvert.DeserializeObject<User>(content); 
                        session.SetString("Username", curr.Name);
                        session.SetInt32("ID", curr.Id);
                        if (curr.Email == "admin@gmail.com")
                            session.SetString("Admin", "True");
                        TempData["success"] = "Login Successful";
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid User");
            }
            TempData["error"] = "Invalid User!";
            return View();
        }
        public IActionResult SignUp()
        {
            if (session.GetInt32("ID") != null)
            {
                session.Clear();
                TempData["warning"] = "You have been signed out!";
                return View();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user)
        {
            if (user != null)
            {
                if (user.Dob > DateTime.Now)
                {
                    TempData["error"] = "Invalid Date of Birth!";
                    return View();
                }
                HttpClient httpClient = new HttpClient();
                StringContent s = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://localhost:7230/api/Users/Signup", s);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Account created successfully";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["error"] = "Couldn't create account!";
            ModelState.AddModelError("","Invalid details!");
            return View();
        }
        public IActionResult SignOut()
        {
            session.Clear();
            TempData["success"] = "You have been logged out successfully!";
            return RedirectToAction("Index", "Home");
        }

    }
}
