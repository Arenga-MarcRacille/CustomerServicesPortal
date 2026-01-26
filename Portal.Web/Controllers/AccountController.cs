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
            return View();
        }
    }
}