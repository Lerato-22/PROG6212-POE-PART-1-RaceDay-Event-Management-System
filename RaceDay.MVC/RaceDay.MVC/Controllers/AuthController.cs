using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _api;

        public AuthController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var response = await _api.PostAsync("/api/auth/register", model);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Registration failed: " + error);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var response = await _api.PostAsync("/api/auth/login", model);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserSession>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user != null)
                {
                    SessionHelper.SetUser(HttpContext.Session, user);

                    // Forward the session cookie from API response back to browser
                    var setCookieHeader = response.Headers
                        .FirstOrDefault(h => h.Key == "Set-Cookie").Value;
                    if (setCookieHeader != null)
                    {
                        foreach (var cookie in setCookieHeader)
                        {
                            Response.Headers.Append("Set-Cookie", cookie);
                        }
                    }

                    if (user.Role == "Organiser")
                        return RedirectToAction("Dashboard", "Organiser");
                    else
                        return RedirectToAction("Index", "Participant");
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _api.PostAsync<object>("/api/auth/logout", null);
            SessionHelper.ClearUser(HttpContext.Session);
            return RedirectToAction("Login");
        }
    }
}
