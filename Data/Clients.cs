using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data
{
    public class Clients
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }

        [Required, StringLength(200)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Assets> Assets { get; set; }
    }
}