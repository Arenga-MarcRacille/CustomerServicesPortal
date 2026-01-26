using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.ViewModels;
using Portal.Services.Interfaces;
using Portal.Data.Data;
using System.Security.Cryptography;
using System.Text;

namespace Portal.Services
{
    public class AccountService : IAccountService
    {
        private readonly PortalWebContext _context;

        public AccountService(PortalWebContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterClientAsync(RegisterViewModel model)
        {
            // 1. Check if user exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                return false;

            // 2. Start a Transaction (Data Integrity!)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Create the User Login
                var newUser = new Users
                {
                    Username = model.Username,
                    Password = HashPassword(model.Password), // Simple Hashing
                    Role = "Client" // Hardcoded because this is public registration
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(); // Generates the UserId

                // B. Create the Client Profile using the new UserId
                var newClient = new Clients // Make sure this matches your class name (Client vs Clients)
                {
                    UserId = newUser.UserId,
                    FullName = model.FullName,
                    Email = model.Email,
                    CreatedAt = DateTime.Now
                };

                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();

                // C. Commit
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            var hashedPassword = HashPassword(password);
            return user.Password == hashedPassword;
        }

        // Helper: SHA256 Hash
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}