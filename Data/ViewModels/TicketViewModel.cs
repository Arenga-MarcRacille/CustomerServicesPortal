using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.ViewModels
{
    public class TicketViewModel
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StatusName { get; set; } // This is now a String ("Open", "Closed")
        public string AssetName { get; set; }
    }
}
