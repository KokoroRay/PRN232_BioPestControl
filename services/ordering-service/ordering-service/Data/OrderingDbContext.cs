using Microsoft.EntityFrameworkCore;
using ordering_service.Models;

namespace ordering_service.Data
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasMany(o => o.OrderItems)
                      .WithOne(i => i.Order)
                      .HasForeignKey(i => i.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                // Index for faster statistics queries
                entity.HasIndex(i => i.ProductId);
            });
        }
    }
}
