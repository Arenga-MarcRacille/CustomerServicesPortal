using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Data; // Check your namespace
using Portal.Services.Interfaces;
using System.Collections.Concurrent; // Needed for Thread-Safe Collections

namespace Portal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly PortalWebContext _context;

        public AdminController(ITicketService ticketService, PortalWebContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        // GET: Admin Dashboard
        public async Task<IActionResult> Index()
        {
            // 1. Fetch all tickets
            var tickets = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.TicketStatus)
                .Include(t => t.Asset)
                .Include(t => t.DocumentRequests) // Needed for attachments
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // 2. PARALLEL PROGRAMMING (Exam Requirement)
            // Meaningful Use-Case: Calculating stats on a potentially large dataset
            var stats = new ConcurrentDictionary<string, int>();
            stats.TryAdd("Open", 0);
            stats.TryAdd("Closed", 0);
            stats.TryAdd("Total", 0);

            Parallel.ForEach(tickets, ticket =>
            {
                // Increment Total
                stats.AddOrUpdate("Total", 1, (key, oldValue) => oldValue + 1);

                // Increment Specific Status
                if (ticket.TicketStatus.StatusName == "Open" || ticket.TicketStatus.StatusName == "InProgress")
                {
                    stats.AddOrUpdate("Open", 1, (key, oldValue) => oldValue + 1);
                }
                else
                {
                    stats.AddOrUpdate("Closed", 1, (key, oldValue) => oldValue + 1);
                }
            });

            // Pass stats to View
            ViewBag.Stats = stats;

            return View(tickets);
        }

        // POST: Update Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int ticketId, int statusId)
        {
            // Call the Service (which calls the Stored Procedure)
            // The SP automatically adds a Timeline entry!
            await _ticketService.UpdateTicketStatusAsync(ticketId, statusId, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }
    }
}