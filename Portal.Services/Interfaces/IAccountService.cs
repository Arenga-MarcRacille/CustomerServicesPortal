using Portal.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterClientAsync(RegisterViewModel model);
        Task<bool> LoginAsync(string username, string password); // We'll need this soon
    }
}
