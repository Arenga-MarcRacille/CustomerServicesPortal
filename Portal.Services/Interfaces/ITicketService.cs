using Portal.Data;
using Portal.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.Interfaces
{
    public interface ITicketService
    {
        // CRUD / SP Operations
        Task<int> CreateTicketAsync(int clientId, int assetId, string title, string description);
        Task<List<TicketViewModel>> GetTicketsByClientAsync(int clientId);
        Task UpdateTicketStatusAsync(int ticketId, int newStatusId, string updaterName);

        // Helpers for UI
        Task<List<Assets>> GetClientAssetsAsync(int clientId);
        Task<Tickets> GetTicketDetailsAsync(int ticketId);

        // Attachment Logic
        Task AddAttachmentAsync(int ticketId, string fileName, string filePath);

        Task AddCommentAsync(int ticketId, int userId, string commentText);
    }
}
