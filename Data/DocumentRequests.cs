using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class DocumentRequests
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Tickets Ticket { get; set; }

        [Required, StringLength(255)]
        public string DocumentName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;
    }
}
