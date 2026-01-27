using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Portal.Data.ViewModels;
using Portal.Services;
using Portal.Services.Interfaces;
using Portal.Web.Models;
using System.Security.Claims;

namespace Portal.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _accountService.RegisterClientAsync(model);
            if (success)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Username already taken.");
            return View(model);
        }

        // GET: Login
        public IActionResult Login()
        {
            // Check if the user is already logged in
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Check if the user is already logged in
                if (User.Identity?.IsAuthenticated == true && User.IsInRole("Client"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Verify credentials using the Service Layer
            if (!await _accountService.LoginAsync(username, password))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View();
            }

            // 2. Define the user's role for redirection logic
            // Usually, you would fetch the actual user object here to get their specific role.
            string role = username.ToLower() == "admin" ? "Admin" : "Client";

            // 3. Create Claims (information about the user to store in the cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Remembers the user after the browser is closed
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // 4. Sign in the user (this creates the encrypted authentication cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 5. Redirect based on the assigned Role
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}