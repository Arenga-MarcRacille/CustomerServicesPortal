using Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.Interfaces
{
    public interface IAssetService
    {
        Task<List<Clients>> GetAllClientsAsync();
        Task<List<Assets>> GetAllAssetsAsync();
        Task CreateAssetAsync(Assets asset);
    }
}
