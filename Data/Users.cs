using System.ComponentModel.DataAnnotations;

namespace Portal.Data
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Client or Admin

        public Clients? Client { get; set; } // Admins might not have a client
    }
}