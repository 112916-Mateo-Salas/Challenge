using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Context
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options) : base (options)
        {
            
        }

        public DbSet<Producto> Productos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>() //Aca configuro los campos de la tabla
                .Property(x => x.Precio)
                .HasColumnType("decimal(18,2)");
        }
    }
}
