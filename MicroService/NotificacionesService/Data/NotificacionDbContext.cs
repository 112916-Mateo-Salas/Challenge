using Microsoft.EntityFrameworkCore;
using NotificacionesService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesService.Data
{
    public class NotificacionDbContext : DbContext
    {
        public NotificacionDbContext(DbContextOptions<NotificacionDbContext> options): base(options) { }

        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
    }
}
