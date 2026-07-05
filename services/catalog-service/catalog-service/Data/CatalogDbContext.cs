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
            modelBuilder.Entity<Crop>().HasData(
                new Crop { Id = 1, Name = "Lúa (Rice)", Slug = "lua", Description = "Cây lương thực chính yếu, dễ gặp sâu đục thân, rầy nâu, đạo ôn.", ImageUrl = "https://images.unsplash.com/photo-1590682680695-43b964a3ae17?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 2, Name = "Cây Ăn Trái", Slug = "cay-an-trai", Description = "Cây xoài, sầu riêng, cam bưởi... Cần nhiều vi lượng để nuôi trái, ra hoa.", ImageUrl = "https://images.unsplash.com/photo-1595163155799-1bd12028ab67?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 3, Name = "Rau Màu", Slug = "rau-mau", Description = "Rau ăn lá, họ dưa bầu bí... dễ gặp sâu tơ, bọ trĩ, phấn trắng.", ImageUrl = "https://images.unsplash.com/photo-1506484381205-f7945653044d?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 4, Name = "Cây Công Nghiệp", Slug = "cay-cong-nghiep", Description = "Cà phê, hồ tiêu, cao su, điều... Các loại cây có giá trị kinh tế cao, cần quản lý nấm bệnh, rệp sáp rễ.", ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 5, Name = "Cây Lấy Củ", Slug = "cay-lay-cu", Description = "Khoai lang, sắn, khoai tây, cà rốt... Cần thuốc trừ tuyến trùng rễ và nhện đỏ.", ImageUrl = "https://images.unsplash.com/photo-1593005872758-00ee9c1e7ea0?q=80&w=600&auto=format&fit=crop", IsActive = true },
                new Crop { Id = 6, Name = "Hoa và Cây Cảnh", Slug = "hoa-cay-canh", Description = "Hoa hồng, lan, cúc... Dễ mắc bệnh rỉ sắt, đốm lá và nhện đỏ.", ImageUrl = "https://images.unsplash.com/photo-1490750967868-88cb4ecb07fa?q=80&w=600&auto=format&fit=crop", IsActive = true }
            );

            // Seed ProductCrops (Map some products to crops)
            modelBuilder.Entity<ProductCrop>().HasData(
                new ProductCrop { ProductId = 50, CropId = 1, UsageInstruction = "Đặc trị rầy nâu, sâu đục thân hại lúa." }, // Regent -> Lúa
                new ProductCrop { ProductId = 47, CropId = 1, UsageInstruction = "Đặc trị bệnh đạo ôn trên lúa." }, // KEEP 300SC -> Lúa
                new ProductCrop { ProductId = 3,  CropId = 1, UsageInstruction = "Trừ ốc bươu vàng hại lúa non." }, // TT SNAILTA -> Lúa
                new ProductCrop { ProductId = 8,  CropId = 1, UsageInstruction = "Giữ xanh lá đòng, hạt lúa sáng mẩy." }, // TT BIOBECA -> Lúa

                new ProductCrop { ProductId = 1,  CropId = 2, UsageInstruction = "Cung cấp Bo và Kẽm giúp đậu trái, chống rụng hoa." }, // Vi lượng -> Trái
                new ProductCrop { ProductId = 4,  CropId = 2, UsageInstruction = "Kích thích ra hoa sớm, đồng loạt." }, // TANO_606 -> Trái
                new ProductCrop { ProductId = 22, CropId = 2, UsageInstruction = "Cung cấp dinh dưỡng NPK nuôi trái lớn." }, // NPK HÀN VIỆT -> Trái
                new ProductCrop { ProductId = 60, CropId = 2, UsageInstruction = "Phòng trị bệnh thán thư sầu riêng, nứt thân xì mủ." }, // PYROLAX -> Trái
                
                new ProductCrop { ProductId = 2,  CropId = 3, UsageInstruction = "Thuốc sinh học an toàn trừ sâu tơ bắp cải." }, // TT-ANONIN -> Rau màu
                new ProductCrop { ProductId = 78, CropId = 3, UsageInstruction = "Đặc trị bệnh phấn trắng trên dưa leo, bầu bí." }, // SULOX -> Rau màu
                new ProductCrop { ProductId = 9,  CropId = 3, UsageInstruction = "Kích rễ rau màu phát triển mạnh." }, // SPC_MKP -> Rau màu
                
                new ProductCrop { ProductId = 24, CropId = 4, UsageInstruction = "Phân bón đa năng giúp phục hồi cây cà phê sau thu hoạch." }, // NPK FERTIGONIA -> Cây CN
                new ProductCrop { ProductId = 81, CropId = 4, UsageInstruction = "Trừ cỏ dại trong vườn hồ tiêu, cà phê." }, // YOSKY -> Cây CN
                
                new ProductCrop { ProductId = 41, CropId = 5, UsageInstruction = "Trừ nấm bệnh thối củ khoai." }, // ZINEB BUL -> Cây Củ
                
                new ProductCrop { ProductId = 17, CropId = 6, UsageInstruction = "Tăng khả năng bám dính thuốc trên lá hoa hồng." } // SAGO BÁM DÍNH -> Hoa
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
