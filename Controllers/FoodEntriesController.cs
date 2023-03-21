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
    public class FoodEntriesController : Controller
    {
        private readonly ISession session;

        public FoodEntriesController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        // GET: FoodEntries
        public async Task<IActionResult> Index()
        {
            if (session.GetInt32("ID") == null)
                return Unauthorized();
            int id = (int)session.GetInt32("ID");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/FoodEntries/getFoodList?uid="+id);
            if (response == null)
            {
                return RedirectToAction("Index","Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                IEnumerable<FoodEntry> foodEntries = JsonConvert.DeserializeObject<IEnumerable<FoodEntry>>(s);
                return View(foodEntries);
            }
            return Unauthorized();
        }

        // GET: FoodEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/FoodEntries/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                FoodEntry foodEntry = new();
                foodEntry = JsonConvert.DeserializeObject<FoodEntry>(s);
                if (foodEntry == null)
                {
                    return NotFound();
                }
                return View(foodEntry);
            }         

            return View();
        }

        // GET: FoodEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Date,Calories,Proteins,Carbs,Fats")] FoodEntry foodEntry)
        {
            if (session.GetInt32("ID") == null)
                return Unauthorized();
            if (foodEntry !=null)
            {
                try
                {
                    foodEntry.UserId = (int)session.GetInt32("ID");
                    HttpClient client = new HttpClient();
                    StringContent s = new StringContent(JsonConvert.SerializeObject(foodEntry), System.Text.Encoding.UTF8, "application/json");
                    var response = client.PostAsync("https://localhost:7230/api/FoodEntries", s);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error occurred while attempting to add entry!";
                    return View(foodEntry);
                }
            }
            TempData["error"] = "Error occurred while attempting to add entry!";
            return View(foodEntry);
        }

        // GET: FoodEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/FoodEntries/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                FoodEntry foodEntry = new();
                foodEntry = JsonConvert.DeserializeObject<FoodEntry>(s);
                if (foodEntry == null)
                {
                    return NotFound();
                }
                return View(foodEntry);
            }
            return NotFound();
        }

        // POST: FoodEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId,Quantity,Date,Calories,Proteins,Carbs,Fats")] FoodEntry foodEntry)
        {
            if (id != foodEntry.Id)
            {
                return NotFound();
            }
            if (foodEntry.Date == null)
                foodEntry.Date = DateTime.Now;
            if (foodEntry.Date > DateTime.Now)
            {
                TempData["error"] = "Date cannot be in the future!";
                return View(foodEntry);
            }

            if (foodEntry != null)
            {
                foodEntry.UserId = (int)session.GetInt32("ID");
                HttpClient client = new HttpClient();
                StringContent s = new StringContent(JsonConvert.SerializeObject(foodEntry), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync("https://localhost:7230/api/FoodEntries/" + id, s);
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
            return View(foodEntry);
        }

        // GET: FoodEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7230/api/FoodEntries/" + id);
            if (response == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                string s = await response.Content.ReadAsStringAsync();
                FoodEntry foodEntry = new();
                foodEntry = JsonConvert.DeserializeObject<FoodEntry>(s);
                if (foodEntry == null)
                {
                    return NotFound();
                }
                return View(foodEntry);
            }
            return NotFound();
        }

        // POST: FoodEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != null)
            {
                HttpClient client = new HttpClient();
                var response = await client.DeleteAsync("https://localhost:7230/api/FoodEntries/" + id);
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
