using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data
{
    public enum TicketStatusEnum
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public class TicketStatus
    {
        [Key]
        public int StatusId { get; set; }
        [Required, StringLength(50)]
        public string StatusName { get; set; }


    }
}
