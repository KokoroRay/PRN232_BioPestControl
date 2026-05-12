using Microsoft.EntityFrameworkCore;
using trading_service.Models;

namespace trading_service.Data
{
    public class TradingDbContext : DbContext
    {
        public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Discount>(e =>
            {
                e.ToTable("Discounts");
                e.HasIndex(d => d.ProductId);
                e.Property(d => d.Name).HasMaxLength(200);
            });

            // Cấu hình quan hệ 1-N giữa Cart và CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Đảm bảo mỗi Customer chỉ có 1 Cart (Unique Index)
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CustomerId)
                .IsUnique();
        }
    }
}
