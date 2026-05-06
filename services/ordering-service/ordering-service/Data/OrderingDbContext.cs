using Microsoft.EntityFrameworkCore;
using ordering_service.Models;

namespace ordering_service.Data
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

        // Bảng giỏ hàng
        public DbSet<Cart> Carts { get; set; }

        // Bảng dòng sản phẩm trong giỏ
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart>(entity =>
            {
                // Mỗi Customer chỉ có đúng 1 giỏ hàng — index unique trên CustomerId
                entity.HasIndex(c => c.CustomerId).IsUnique();

                // Cascade delete: khi xóa Cart thì xóa toàn bộ CartItem của nó
                entity.HasMany(c => c.Items)
                      .WithOne(i => i.Cart)
                      .HasForeignKey(i => i.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                // Mỗi sản phẩm chỉ xuất hiện 1 lần trong cùng một giỏ
                entity.HasIndex(i => new { i.CartId, i.ProductId }).IsUnique();
            });
        }
    }
}
