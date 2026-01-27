using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Data.Data;
using Portal.Data.ViewModels;
using Portal.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Portal.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly PortalWebContext _context;

        public TicketsController(ITicketService ticketService, PortalWebContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var clientId = await GetCurrentClientId();
            var assets = await _ticketService.GetClientAssetsAsync(clientId);

            ViewBag.AssetId = new SelectList(assets, "AssetId", "AssetName");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            var clientId = await GetCurrentClientId();

            if (ModelState.IsValid)
            {
                // 1. Create Ticket via Stored Procedure
                int newTicketId = await _ticketService.CreateTicketAsync(
                    clientId, model.AssetId, model.Title, model.Description);

                // 2. Handle Document Upload (Requirement: Uploading of Documents)
                if (model.Attachment != null)
                {
                    var fileName = Guid.NewGuid().ToString() + "_" + model.Attachment.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }

                    await _ticketService.AddAttachmentAsync(newTicketId, model.Attachment.FileName, "/uploads/" + fileName);
                }

                return RedirectToAction("Index", "Dashboard");
            }

            var assets = await _ticketService.GetClientAssetsAsync(clientId);
            ViewBag.AssetId = new SelectList(assets, "AssetId", "AssetName");
            return View(model);
        }

        // GET: Details (Requirement: Ticket Details with Timeline)
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Details(int id)
        {
            // 1. Fetch the ticket with all includes (Asset, Client, Status, etc.)
            var ticket = await _ticketService.GetTicketDetailsAsync(id);

            // 2. Global Null Check: If it doesn't exist in DB, nobody sees it.
            if (ticket == null)
            {
                return NotFound();
            }

            // 3. Security Check: If the user is a Client, they must OWN this ticket.
            if (User.IsInRole("Client"))
            {
                // Get the logged-in User's Name (or ID if you have it in claims)
                var currentUsername = User.Identity.Name;

                // Verify if the ticket's client username matches the logged-in user
                if (ticket.Client.User.Username != currentUsername)
                {
                    return Forbid(); // Or return NotFound() if you want to be sneaky
                }
            }

            // 4. Admins pass through automatically because of the [Authorize] attribute 
            // and the fact that we don't restrict them by ClientId.
            return View(ticket);
        }

        private async Task<int> GetCurrentClientId()
        {
            var username = User.Identity?.Name;
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.User.Username == username);
            return client?.ClientId ?? 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int ticketId, string commentText)
        {
            // 1. Basic Validation
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return RedirectToAction("Details", new { id = ticketId });
            }

            // 2. Identify the Author
            var username = User.Identity.Name;

            // We need the User object to get the UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                // 3. Save via Service
                await _ticketService.AddCommentAsync(ticketId, user.UserId, commentText);
            }

            // 4. Reload the page so they see the new comment
            return RedirectToAction("Details", new { id = ticketId });
        }
    }
}