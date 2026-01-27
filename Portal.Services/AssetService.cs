using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Data; // Check your namespace for Context
using Portal.Services.Interfaces;

namespace Portal.Services
{
    public class AssetService : IAssetService
    {
        private readonly PortalWebContext _context;

        public AssetService(PortalWebContext context)
        {
            _context = context;
        }

        public async Task<List<Clients>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<List<Assets>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Include(a => a.Client) // Include Client to show owner name
                .OrderByDescending(a => a.AssetId)
                .ToListAsync();
        }

        public async Task CreateAssetAsync(Assets asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
        }
    }
}