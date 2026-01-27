using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public int AssetId { get; set; } // Dropdown selection

        // Requirement: "Create Ticket and Uploading of Documents"
        [Display(Name = "Attachment (Optional)")]
        public IFormFile? Attachment { get; set; }
    }
}
