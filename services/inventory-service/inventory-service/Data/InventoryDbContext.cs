using inventory_service.Models;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<WarehouseImport> WarehouseImports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint: mỗi sản phẩm có SKU không trùng
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            // Relationship: một Product có nhiều WarehouseImport
            modelBuilder.Entity<WarehouseImport>()
                .HasOne(wi => wi.Product)
                .WithMany(p => p.WarehouseImports)
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data: một vài sản phẩm mẫu
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    SKU = "BPC-001",
                    Name = "Thuốc trừ sâu Sinh học BT",
                    Description = "Thuốc trừ sâu sinh học chiết xuất từ vi khuẩn Bacillus thuringiensis",
                    Unit = "Lít",
                    StockQuantity = 100,
                    LowStockThreshold = 20,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    SKU = "BPC-002",
                    Name = "Thuốc trừ nấm Đồng hữu cơ",
                    Description = "Thuốc trừ bệnh phổ rộng, an toàn với môi trường",
                    Unit = "Kg",
                    StockQuantity = 50,
                    LowStockThreshold = 10,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 3,
                    SKU = "BPC-003",
                    Name = "Phân bón lá hữu cơ",
                    Description = "Phân bón hữu cơ dạng lỏng bổ sung dinh dưỡng cho cây trồng",
                    Unit = "Lít",
                    StockQuantity = 200,
                    LowStockThreshold = 30,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
