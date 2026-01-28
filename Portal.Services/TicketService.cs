using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Data;
using Portal.Data.ViewModels;
using Portal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Portal.Services
{
    public class TicketService : ITicketService
    {
        private readonly PortalWebContext _context;

        public TicketService(PortalWebContext context)
        {
            _context = context;
        }

        public async Task<int> CreateTicketAsync(int clientId, int assetId, string title, string description)
        {
            var p1 = new SqlParameter("@ClientId", clientId);
            var p2 = new SqlParameter("@AssetId", assetId);
            var p3 = new SqlParameter("@Title", title);
            var p4 = new SqlParameter("@Description", description);

            // Calling sp_CreateTicket which returns the new TicketId
            var result = await _context.Database
                .SqlQueryRaw<int>("EXEC sp_CreateTicket @ClientId, @AssetId, @Title, @Description", p1, p2, p3, p4)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<List<TicketViewModel>> GetTicketsByClientAsync(int clientId)
        {
            var p1 = new SqlParameter("@ClientId", clientId);

            // Calling sp_GetClientTickets which joins Tickets, Assets, and Status
            return await _context.Database
                .SqlQueryRaw<TicketViewModel>("EXEC sp_GetClientTickets @ClientId", p1)
                .ToListAsync();
        }

        public async Task UpdateTicketStatusAsync(int ticketId, int newStatusId, string updaterName)
        {
            var p1 = new SqlParameter("@TicketId", ticketId);
            var p2 = new SqlParameter("@NewStatusId", newStatusId);
            var p3 = new SqlParameter("@UpdaterName", updaterName);

            // Calling sp_UpdateTicketStatus (handles the UPDATE and the Timeline INSERT)
            await _context.Database
                .ExecuteSqlRawAsync("EXEC sp_UpdateTicketStatus @TicketId, @NewStatusId, @UpdaterName", p1, p2, p3);
        }

        // --- Helper Methods ---

        public async Task<List<Assets>> GetClientAssetsAsync(int clientId)
        {
            return await _context.Assets
                .Where(a => a.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<Tickets> GetTicketDetailsAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Asset)
                .Include(t => t.TicketStatus)
                .Include(t => t.Timelines)
                .Include(t => t.Client)
                .ThenInclude(c => c.User)
                .Include(t => t.DocumentRequests)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            return ticket;
        }

        public async Task AddAttachmentAsync(int ticketId, string fileName, string filePath)
        {
            var attachment = new DocumentRequests
            {
                TicketId = ticketId,
                DocumentName = fileName,
                FilePath = filePath,
                RequestedAt = DateTime.Now
            };

            _context.DocumentRequests.Add(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(int ticketId, int userId, string commentText)
        {
            var comment = new TicketComments
            {
                TicketId = ticketId,
                UserId = userId,
                CommentText = commentText,
                CreatedAt = DateTime.Now
            };

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}