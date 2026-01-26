using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class Tickets
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        public int AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Assets Asset { get; set; }

        [Required]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Clients Client { get; set; }

        [Required]
        public int TicketStatusId { get; set; }
        [ForeignKey("TicketStatusId")]
        public TicketStatus TicketStatus { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<TicketComments> Comments { get; set; }
        public virtual ICollection<TicketTimelines> Timelines { get; set; }
        public virtual ICollection<DocumentRequests> DocumentRequests { get; set; }
    }
}

