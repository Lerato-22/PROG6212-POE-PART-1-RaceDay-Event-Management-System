using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _api;

        public ParticipantController(ApiService api)
        {
            _api = api;
        }

        private IActionResult RedirectIfNotParticipant()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Login", "Auth");
            if (!SessionHelper.IsParticipant(HttpContext.Session))
                return RedirectToAction("Dashboard", "Organiser");
            return null;
        }

        // Home - all upcoming events
        public async Task<IActionResult> Index(string search, string eventType)
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            var events = await _api.GetAsync<List<EventModel>>("/api/events");
            if (!string.IsNullOrEmpty(search))
                events = events?.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrEmpty(eventType))
                events = events?.Where(e => e.EventType == eventType).ToList();

            ViewBag.Search = search;
            ViewBag.EventType = eventType;
            return View(events ?? new List<EventModel>());
        }

        // Event detail + enrol
        public async Task<IActionResult> EventDetail(int id)
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            var ev = await _api.GetAsync<EventModel>($"/api/events/{id}");
            var categories = await _api.GetAsync<List<CategoryModel>>($"/api/events/{id}/categories");
            ViewBag.Categories = categories ?? new List<CategoryModel>();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(EnrolmentCreateModel model)
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            await _api.PostAsync("/api/enrolments", model);
            return RedirectToAction("MyEvents");
        }

        // My Events
        public async Task<IActionResult> MyEvents()
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            var enrolments = await _api.GetAsync<List<EnrolmentModel>>("/api/enrolments/my");
            return View(enrolments ?? new List<EnrolmentModel>());
        }

        // My Results
        public async Task<IActionResult> MyResults()
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            var results = await _api.GetAsync<List<ResultModel>>("/api/results/my");
            return View(results ?? new List<ResultModel>());
        }

        // Profile
        public async Task<IActionResult> Profile()
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            var profile = await _api.GetAsync<ProfileModel>("/api/profile");
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileUpdateModel model)
        {
            var check = RedirectIfNotParticipant();
            if (check != null) return check;

            await _api.PutAsync("/api/profile", new
            {
                firstName = model.FullName?.Split(' ')[0] ?? "",
                lastName = model.FullName?.Contains(' ') == true ? model.FullName.Split(' ', 2)[1] : "",
                phoneNumber = model.PhoneNumber
            });

            return RedirectToAction("Profile");
        }
    }
}
