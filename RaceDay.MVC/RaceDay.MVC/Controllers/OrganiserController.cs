using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _api;

        public OrganiserController(ApiService api)
        {
            _api = api;
        }

        private IActionResult RedirectIfNotOrganiser()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Login", "Auth");
            if (!SessionHelper.IsOrganiser(HttpContext.Session))
                return RedirectToAction("Index", "Participant");
            return null;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var events = await _api.GetAsync<List<EventModel>>("/api/events/my");
            return View(events ?? new List<EventModel>());
        }

        // Events
        public async Task<IActionResult> Events()
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var events = await _api.GetAsync<List<EventModel>>("/api/events/my");
            return View(events ?? new List<EventModel>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventCreateModel model, IFormFile? BannerImage)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Name ?? ""), "eventName");
            formData.Add(new StringContent(model.Description ?? ""), "description");
            formData.Add(new StringContent(model.Date.ToString("o")), "eventDate");
            formData.Add(new StringContent(model.Location ?? ""), "location");
            formData.Add(new StringContent(model.Distance.ToString()), "distance");
            formData.Add(new StringContent(model.EventType ?? ""), "eventType");

            if (BannerImage != null && BannerImage.Length > 0)
            {
                var stream = BannerImage.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(BannerImage.ContentType);
                formData.Add(fileContent, "bannerImage", BannerImage.FileName);
            }

            var response = await _api.PostFormAsync("/api/events", formData);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Events");

            ModelState.AddModelError("", "Failed to create event.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var ev = await _api.GetAsync<EventModel>($"/api/events/{id}");
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, EventModel model, IFormFile? BannerImage)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Name ?? ""), "eventName");
            formData.Add(new StringContent(model.Description ?? ""), "description");
            formData.Add(new StringContent(model.Date.ToString("o")), "eventDate");
            formData.Add(new StringContent(model.Location ?? ""), "location");
            formData.Add(new StringContent(model.Distance.ToString()), "distance");
            formData.Add(new StringContent(model.EventType ?? ""), "eventType");

            if (BannerImage != null && BannerImage.Length > 0)
            {
                var stream = BannerImage.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(BannerImage.ContentType);
                formData.Add(fileContent, "bannerImage", BannerImage.FileName);
            }

            var response = await _api.PutFormAsync($"/api/events/{id}", formData);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Events");

            ModelState.AddModelError("", "Failed to update event.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            await _api.DeleteAsync($"/api/events/{id}");
            return RedirectToAction("Events");
        }

        // Categories
        public async Task<IActionResult> Categories(int eventId)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var categories = await _api.GetAsync<List<CategoryModel>>($"/api/events/{eventId}/categories");
            ViewBag.EventId = eventId;
            return View(categories ?? new List<CategoryModel>());
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, CategoryCreateModel model)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            await _api.PostAsync($"/api/events/{eventId}/categories", model);
            return RedirectToAction("Categories", new { eventId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int eventId, int id)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            await _api.DeleteAsync($"/api/events/{eventId}/categories/{id}");
            return RedirectToAction("Categories", new { eventId });
        }

        // Enrolments
        public async Task<IActionResult> Enrolments(int eventId)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var enrolments = await _api.GetAsync<List<EnrolmentModel>>($"/api/events/{eventId}/enrolments");
            ViewBag.EventId = eventId;
            return View(enrolments ?? new List<EnrolmentModel>());
        }

        // Results
        public async Task<IActionResult> Results(int eventId)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            var results = await _api.GetAsync<List<ResultModel>>($"/api/events/{eventId}/results");
            var enrolments = await _api.GetAsync<List<EnrolmentModel>>($"/api/events/{eventId}/enrolments");
            ViewBag.EventId = eventId;
            ViewBag.Enrolments = enrolments ?? new List<EnrolmentModel>();
            return View(results ?? new List<ResultModel>());
        }

        [HttpPost]
        public async Task<IActionResult> AddResult(ResultCreateModel model)
        {
            var check = RedirectIfNotOrganiser();
            if (check != null) return check;

            await _api.PostAsync("/api/results", model);
            return RedirectToAction("Results", new { eventId = TempData["EventId"] });
        }
    }
}