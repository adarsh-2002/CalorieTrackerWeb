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
    public class WorkoutsController : Controller
    {
        private readonly ISession session;

        public WorkoutsController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            if (session.GetInt32("ID") == null)
                return Unauthorized();
            int id = (int)session.GetInt32("ID");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/Workouts?uid=" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                IEnumerable<Workout> workouts = JsonConvert.DeserializeObject<IEnumerable<Workout>>(s);
                return View(workouts);
            }
            return Unauthorized();
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/Workouts/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                Workout workout = new();
                workout = JsonConvert.DeserializeObject<Workout>(s);
                if (workout == null)
                {
                    return NotFound();
                }
                return View(workout);
            }

            return View();
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,WDateTime,Name,Calories")] Workout workout)
        {
            if (session.GetInt32("ID") == null)
                return Unauthorized();
            if (workout != null)
            {
                try
                {
                    workout.UserId = (int)session.GetInt32("ID");
                    HttpClient client = new HttpClient();
                    StringContent s = new StringContent(JsonConvert.SerializeObject(workout), System.Text.Encoding.UTF8, "application/json");
                    var response = client.PostAsync("https://localhost:7230/api/Workouts", s);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error occurred while attempting to add entry!";
                    return View(workout);
                }
            }
            TempData["error"] = "Error occurred while attempting to add entry!";
            return View(workout);
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/Workouts/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                Workout workout = new();
                workout = JsonConvert.DeserializeObject<Workout>(s);
                if (workout == null)
                {
                    return NotFound();
                }
                return View(workout);
            }
            return NotFound();
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,WDateTime,Name,Calories")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }
            if (workout.WDateTime == null)
                workout.WDateTime = DateTime.Now;
            if (workout.WDateTime > DateTime.Now)
            {
                TempData["error"] = "Date cannot be in the future!";
                return View(workout);
            }

            if (workout != null)
            {
                workout.UserId = (int)session.GetInt32("ID");
                HttpClient client = new HttpClient();
                StringContent s = new StringContent(JsonConvert.SerializeObject(workout), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync("https://localhost:7230/api/Workouts/" + id, s);
                if (response == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Details updated successfully";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "An error occurred!";
            return View(workout);
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/Workouts/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                Workout workout = new();
                workout = JsonConvert.DeserializeObject<Workout>(s);
                if (workout == null)
                {
                    return NotFound();
                }
                return View(workout);
            }
            return NotFound();
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != null)
            {
                HttpClient client = new HttpClient();
                var response = await client.DeleteAsync("https://localhost:7230/api/Workouts/" + id);
                if (response == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Entry deleted successfully";
                    return View("Index");
                }
            }
            TempData["error"] = "An error occurred!";
            return RedirectToAction("Index");
        }
    }
}
