using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class TicketTimelines
    {
        [Key]
        public int TimelineId { get; set; }

        [Required]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Tickets Ticket { get; set; }

        [Required]
        public string ActivityDescription { get; set; }

        public DateTime LogDate { get; set; } = DateTime.Now;
    }
}
