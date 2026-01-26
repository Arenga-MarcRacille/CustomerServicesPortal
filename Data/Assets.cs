using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class Assets
    {
        [Key]
        public int AssetId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Clients Client { get; set; }

        [Required, StringLength(100)]
        public string AssetName { get; set; }

        [Required]
        public string AssetType { get; set; }
    }
}
