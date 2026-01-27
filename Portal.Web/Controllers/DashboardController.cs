using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Data;
using Portal.Services;
using Portal.Services.Interfaces;
using Portal.Web.Models; // Ensure this matches your ViewModel namespace
using System.Security.Claims;

namespace Portal.Web.Controllers
{
    [Authorize] // Ensures only logged-in users can access
    public class DashboardController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly PortalWebContext _context;

        public DashboardController(ITicketService ticketService, PortalWebContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Identify the current user
            // Assuming you set ClaimTypes.Name to the Username during login
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Resolve ClientId from the Database
            // We need to know WHICH client profile belongs to this user.
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Username == username);

            if (client == null)
            {
                // Edge case: User is logged in (maybe as Admin) but has no Client profile
                return View("Error", new ErrorViewModel { RequestId = "User has no Client Profile" });
            }

            // 3. Call the Service (which calls the Stored Procedure)
            var tickets = await _ticketService.GetTicketsByClientAsync(client.ClientId);

            // 4. Return the View with data
            return View(tickets);
        }
    }
}