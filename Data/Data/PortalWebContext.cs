using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portal.Data;

namespace Portal.Data.Data
{
    public class PortalWebContext : DbContext
    {
        public PortalWebContext (DbContextOptions<PortalWebContext> options)
            : base(options)
        {
        }

        public DbSet<Portal.Data.Users> Users { get; set; } = default!;
        public DbSet<Portal.Data.Clients> Clients { get; set; } = default!;
        public DbSet<Portal.Data.Assets> Assets { get; set; } = default!;
        public DbSet<Portal.Data.Tickets> Tickets { get; set; } = default!;
        public DbSet<Portal.Data.TicketComments> TicketComments { get; set; } = default!;
        public DbSet<Portal.Data.TicketStatus> TicketStatus { get; set; } = default!;
        public DbSet<Portal.Data.TicketTimelines> TicketTimelines { get; set; } = default!;
        public DbSet<Portal.Data.DocumentRequests> DocumentRequests { get; set; } = default!;


    }
}
