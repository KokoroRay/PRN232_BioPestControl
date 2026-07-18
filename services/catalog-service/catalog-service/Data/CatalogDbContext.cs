using catalog_service.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace catalog_service.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Crop> Crops { get; set; }
        public DbSet<ProductCrop> ProductCrops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phân bón và hóa chất" },
                new Category { Id = 2, Name = "Thuốc trừ bệnh" },
                new Category { Id = 3, Name = "Thuốc trừ cỏ" },
                new Category { Id = 4, Name = "Thuốc trừ sâu" }
            );

            // Seed Crops (Mở rộng nhiều loại cây trồng)
            modelBuilder.SeedProducts();
            modelBuilder.SeedProductCrops();
                        modelBuilder.Entity<Crop>().HasData(
                new Crop { Id = 1, Name = "Lúa (Rice)", Slug = "lua", Description = "Cây lương thực chính yếu, dễ gặp sâu đục thân, rầy nâu, đạo ôn.", ImageUrl = "https://images.unsplash.com/photo-1590682680695-43b964a3ae17?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 2, Name = "Cây Ăn Trái", Slug = "cay-an-trai", Description = "Cây xoài, sầu riêng, cam bưởi... Cần nhiều vi lượng để nuôi trái, ra hoa.", ImageUrl = "https://images.unsplash.com/photo-1595163155799-1bd12028ab67?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 3, Name = "Rau Màu", Slug = "rau-mau", Description = "Rau ăn lá, họ dưa bầu bí... dễ gặp sâu tơ, bọ trĩ, phấn trắng.", ImageUrl = "https://images.unsplash.com/photo-1506484381205-f7945653044d?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 4, Name = "Cây Công Nghiệp", Slug = "cay-cong-nghiep", Description = "Cà phê, hồ tiêu, cao su, điều... Các loại cây có giá trị kinh tế cao, cần quản lý nấm bệnh, rệp sáp rễ.", ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 5, Name = "Cây Lấy Củ", Slug = "cay-lay-cu", Description = "Khoai lang, sắn, khoai tây, cà rốt... Cần thuốc trừ tuyến trùng rễ và nhện đỏ.", ImageUrl = "https://images.unsplash.com/photo-1593005872758-00ee9c1e7ea0?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 6, Name = "Hoa và Cây Cảnh", Slug = "hoa-cay-canh", Description = "Hoa hồng, lan, cúc... Dễ mắc bệnh rỉ sắt, đốm lá và nhện đỏ.", ImageUrl = "https://images.unsplash.com/photo-1490750967868-88cb4ecb07fa?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 7, Name = "Cà Phê", Slug = "ca-phe", Description = "Cây công nghiệp dài ngày, rất dễ nhiễm rệp sáp, tuyến trùng, bệnh gỉ sắt và nấm hồng.", ImageUrl = "https://images.unsplash.com/photo-1511920170033-f8396924c348?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 8, Name = "Sầu Riêng", Slug = "sau-rieng", Description = "Cây ăn trái có giá trị cao, thường mắc bệnh nứt thân xì mủ do Phytophthora, rầy phấn, nhện đỏ.", ImageUrl = "https://res.cloudinary.com/biopestcontrol/image/upload/v1700000001/durian.jpg", IsActive = true },
                new Crop { Id = 9, Name = "Hồ Tiêu", Slug = "ho-tieu", Description = "Cây công nghiệp dễ bị bệnh chết nhanh, chết chậm do nấm Phytophthora và tuyến trùng rễ.", ImageUrl = "https://res.cloudinary.com/biopestcontrol/image/upload/v1700000002/pepper.jpg", IsActive = true },
                new Crop { Id = 10, Name = "Xoài", Slug = "xoai", Description = "Thường bị bệnh thán thư hại bông và trái, rầy xanh, rệp sáp, cần phun thuốc định kỳ lúc ra hoa.", ImageUrl = "https://images.unsplash.com/photo-1553284965-83fd3e82fa5a?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 11, Name = "Bắp Cải", Slug = "bap-cai", Description = "Rau ăn lá rất dễ bị sâu tơ, sâu xanh bướm trắng, bệnh thối nhũn vi khuẩn.", ImageUrl = "https://images.unsplash.com/photo-1518972554746-b31c195f19e4?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 12, Name = "Cam, Bưởi", Slug = "cam-buoi", Description = "Nhóm cây có múi, thường bị sâu vẽ bùa, rầy chổng cánh, nhện đỏ và bệnh vàng lá gân xanh.", ImageUrl = "https://images.unsplash.com/photo-1558293674-1e0e854497e6?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 13, Name = "Ngô (Bắp)", Slug = "ngo", Description = "Cây lương thực ngắn ngày, thường bị sâu keo mùa thu, rệp cờ, bệnh khô vằn.", ImageUrl = "https://images.unsplash.com/photo-1551754655-cd27e38d2076?q=80&w=600&auto=format&fit=crop", IsActive = true }
            );



            modelBuilder.Entity<ProductCrop>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CropId });
                entity.HasOne(pc => pc.Product)
                      .WithMany(p => p.ProductCrops)
                      .HasForeignKey(pc => pc.ProductId);
                entity.HasOne(pc => pc.Crop)
                      .WithMany(c => c.ProductCrops)
                      .HasForeignKey(pc => pc.CropId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.SKU).IsUnique();
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

