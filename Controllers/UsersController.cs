using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CalorieTrackerWeb.Models;
using Newtonsoft.Json;

namespace CalorieTrackerWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ISession session;

        public UsersController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if(session.GetString("Admin") == "True")
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://localhost:7230/api/Users");
                if(response != null)
                {
                    string s = await response.Content.ReadAsStringAsync();
                    IEnumerable<User> users = JsonConvert.DeserializeObject<User[]>(s);
                    return View(users);
                }
            }
            TempData["error"] = "Unauthorised to access resource!";
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details()
        {
            if(session.GetInt32("ID") != null)
            {
                int id = (int)session.GetInt32("ID");
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://localhost:7230/api/Users/Details?id=" +id);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string s = await response.Content.ReadAsStringAsync();
                        User user = new User();
                        user = JsonConvert.DeserializeObject<User>(s);
                        return View(user);
                    }
                }
            }
            TempData["error"] = "An error occurred";
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit()
        {
            if (session.GetInt32("ID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://localhost:7230/api/Users/Details?id=" + session.GetInt32("ID"));
            if(response != null)
            {
                string s = await response.Content.ReadAsStringAsync();
                User user = new User();
                user = JsonConvert.DeserializeObject<User>(s);
                return View(user);
            }
            return NotFound();
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Email,Password,Dob,Gender,Height,Weight,RequiredCalories")] User user)
        {
            if (session.GetInt32("ID") != user.Id)
            {
                return NotFound();
            }
            HttpClient httpClient = new HttpClient();
            StringContent s = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync("https://localhost:7230/api/Users/" + session.GetInt32("ID"), s);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Details updated successfully";
                    return RedirectToAction("Details");
                }
            }
            TempData["error"] = "An Error occurred!";
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete()
        {
            if (session.GetInt32("id") == null)
            {
                return NotFound();
            }

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://localhost:7230/api/Users/Details?id=" + session.GetInt32("ID"));
            if (response != null)
            {
                string s = await response.Content.ReadAsStringAsync();
                User user = new User();
                user = JsonConvert.DeserializeObject<User>(s);
                return View(user);
            }
            TempData["error"] = "Please login to delete user!";
            return RedirectToAction("Index", "Login");
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync("https://localhost:7230/api/Users/" + id);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    session.Clear();
                    TempData["success"] = "Your account has been deleted and you have been logged out!";
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Details", "Users");
        }

    }
}
