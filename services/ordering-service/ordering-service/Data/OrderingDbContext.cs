using Microsoft.EntityFrameworkCore;

namespace ordering_service.Data
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

        // Future tables for ordering-service (e.g. Order, OrderDetail) will go here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
