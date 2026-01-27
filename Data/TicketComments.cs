using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portal.Data.Data; // Check namespace for Users class

namespace Portal.Data
{
    public class TicketComments
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Tickets Ticket { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users User { get; set; }

        [Required]
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}