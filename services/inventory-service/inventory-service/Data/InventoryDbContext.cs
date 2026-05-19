using inventory_service.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

            // Seed inventory products based on legacy data (all 83 products)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, SKU = "sp0001", Name = "Vi lượng-BOROZINC", Description = "Bo và Kẽm...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 2, SKU = "sp0002", Name = "TT-ANONIN 1EC", Description = "Thuốc trừ sâu...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 3, SKU = "sp0003", Name = "TT SNAILTA GOLD 750WP", Description = "Công thức mới...", Unit = null, StockQuantity = 49, LowStockThreshold = 10, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 4, SKU = "sp0004", Name = "TANO_606", Description = "Giúp ra hoa...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 5, SKU = "sp0005", Name = "TANO_601", Description = "Giúp cây...", Unit = null, StockQuantity = 10, LowStockThreshold = 5, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 6, SKU = "sp0006", Name = "SUNPHAT_KẼM", Description = "Sunphat...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 7, SKU = "sp0007", Name = "SPC - NPK", Description = "Tăng độ cứng...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 8, SKU = "sp0008", Name = "TT BIOBECA 0.1SP", Description = "TT BIOBECA...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 9, SKU = "sp0009", Name = "SPC_MKP", Description = "Kích thích...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 10, SKU = "sp0010", Name = "SPC_KALI_SILIC", Description = "Hạn chế...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 11, SKU = "sp0011", Name = "SAMINO_51SL", Description = "SAMINO...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 12, SKU = "sp0012", Name = "SAIGON_P115WP", Description = "SAIGON...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 13, SKU = "sp0013", Name = "SAGOLATEX", Description = "SAGOLATEX...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 14, SKU = "sp0014", Name = "SAGO SÓNG THẦN", Description = "Giúp...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 15, SKU = "sp0015", Name = "SAGO ĐỒNG", Description = "SAGO...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 16, SKU = "sp0016", Name = "SAGO SIÊU HẤP", Description = "Đang cập nhật", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 17, SKU = "sp0017", Name = "SAGO BÁM DÍNH", Description = "Tăng khả năng...", Unit = null, StockQuantity = 0, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 18, SKU = "sp0018", Name = "PLASTIMULA 1SL", Description = "Giúp phát triển...", Unit = null, StockQuantity = 30, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 19, SKU = "sp0019", Name = "SAGO AXIT", Description = "Giúp phá vỡ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 20, SKU = "sp0020", Name = "ORGANIC NOKAYO", Description = "Giúp đất...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 21, SKU = "sp0021", Name = "ORGANIC YUKIMOTO", Description = "Bổ sung...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 22, SKU = "sp0022", Name = "NPK HÀN VIỆT 20 20 15 TE", Description = "Cung cấp...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 23, SKU = "sp0023", Name = "NPK HÀN VIỆT 15-15-15", Description = "Thúc đẩy...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 24, SKU = "sp0024", Name = "NPK FERTIGONIA", Description = "FERTIGONIA...", Unit = null, StockQuantity = 47, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 25, SKU = "sp0025", Name = "NATRAZYME", Description = "Natrazyme...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 26, SKU = "sp0026", Name = "NPK 16-16-8-13S", Description = "Cung cấp...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 27, SKU = "sp0027", Name = "SOP", Description = "Tăng sức...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 28, SKU = "sp0028", Name = "NPK 17-7-17 + TE", Description = "Phân bón...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 29, SKU = "sp0029", Name = "NPK 16-16-8 + TE", Description = "Phân NPK...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 30, SKU = "sp0030", Name = "NPK TÂN THÀNH 25-25-5", Description = "Cung cấp...", Unit = null, StockQuantity = 10, LowStockThreshold = 5, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 31, SKU = "sp0031", Name = "NPK TÂN THÀNH 20-20-15 TE", Description = "Phân đa...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 32, SKU = "sp0032", Name = "LACASOTO 4SP", Description = "Thuốc sinh...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 33, SKU = "sp0033", Name = "CHUBECA", Description = "Polyphenol...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 34, SKU = "sp0034", Name = "DAP TÂN THÀNH", Description = "Cung cấp...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 35, SKU = "sp0035", Name = "ATRAZIN", Description = "Cải tạo...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 36, SKU = "sp0036", Name = "KẼM BORON", Description = "Kẽm và Boron...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 37, SKU = "sp0037", Name = "ANIMAT", Description = "Bo và Kẽm...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 38, SKU = "sp0038", Name = "SPC - MKP", Description = "Giúp kích...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 39, SKU = "sp0039", Name = "CHOCASO", Description = "Chocaso...", Unit = null, StockQuantity = 46, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 40, SKU = "sp0040", Name = "NKP TE", Description = "Phân NPK...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 41, SKU = "sp0041", Name = "ZINEB BUL 80WP", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 42, SKU = "sp0042", Name = "ZIN 80 WP", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 43, SKU = "sp0043", Name = "XINAZO", Description = "XINAZO...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 44, SKU = "sp0044", Name = "VANICIDE", Description = "VANICIDE...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 45, SKU = "sp0045", Name = "UNITIL", Description = "Thuốc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 46, SKU = "sp0046", Name = "BASU 250WP", Description = "Thuốc đặc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 47, SKU = "sp0047", Name = "KEEP 300SC", Description = "Công thức...", Unit = null, StockQuantity = 20, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 48, SKU = "sp0048", Name = "ATANIL", Description = "Thuốc...", Unit = null, StockQuantity = 0, LowStockThreshold = 10, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 49, SKU = "sp0049", Name = "BIOMYCIN", Description = "Thuốc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 50, SKU = "sp0050", Name = "Regent", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 51, SKU = "sp0051", Name = "TRIZOLE 400SC", Description = "Trizole...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 52, SKU = "sp0052", Name = "TRIZOLE 75WP", Description = "Trizole...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 53, SKU = "sp0053", Name = "TRIZOLE 75 DO", Description = "Trizole...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 54, SKU = "sp0054", Name = "TRI 75WG", Description = "TRI...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 55, SKU = "sp0055", Name = "TREPPACH BUL", Description = "TREPPACH...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 56, SKU = "sp0056", Name = "SAIPORA", Description = "SAIPORA...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 57, SKU = "sp0057", Name = "SAGOPERFECT 320", Description = "SAGOPERFECT...", Unit = null, StockQuantity = 0, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 58, SKU = "sp0058", Name = "SAGOGRAIN 300EC", Description = "Diệt...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 59, SKU = "sp0059", Name = "ROTEVA 30SC", Description = "ROTEVA...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 60, SKU = "sp0060", Name = "PYROLAX 250EC", Description = "Thuốc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 61, SKU = "sp0061", Name = "OTICIN 47.5WP", Description = "Thuốc phòng...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 62, SKU = "sp0062", Name = "MEXYL MZ", Description = "Thuốc phòng...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 63, SKU = "sp0063", Name = "LUSTER 250SC", Description = "Thuốc có...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 64, SKU = "sp0064", Name = "LUNASA 80", Description = "Giúp...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 65, SKU = "sp0065", Name = "LÚA VÀNG", Description = "Thuốc có...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 66, SKU = "sp0066", Name = "KAISAIGON", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 67, SKU = "sp0067", Name = "KAISAIGON 10", Description = "Thuốc đặc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 68, SKU = "sp0068", Name = "CẢP XANH", Description = "Thuốc phòng...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 69, SKU = "sp0069", Name = "HÒA TIÊN", Description = "HÒA...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 70, SKU = "sp0070", Name = "HẠT VÀNG", Description = "HẠT...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 71, SKU = "sp0071", Name = "EDIVIL 80WP", Description = "Edivil...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 72, SKU = "sp0072", Name = "DIPOMATE", Description = "DIPOMATE...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 73, SKU = "sp0073", Name = "DIPOMATE 80", Description = "Phòng...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 74, SKU = "sp0074", Name = "CLEARNER 75 WP", Description = "CLEARNER...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 75, SKU = "sp0075", Name = "CHUBECA 1.8SL", Description = "Đặc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 76, SKU = "sp0076", Name = "ALPINE 80WP", Description = "ALPINE...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 77, SKU = "sp0077", Name = "ALPINE XANH", Description = "ALPINE...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 78, SKU = "sp0078", Name = "SULOX 80WP", Description = "Đặc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 79, SKU = "sp0079", Name = "SAIZOLE 5SC", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 80, SKU = "sp0080", Name = "SAIPAN 2SL", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 81, SKU = "sp0081", Name = "YOSKY", Description = "Thuốc trừ...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 82, SKU = "sp0082", Name = "XINRON", Description = "Thuốc...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 83, SKU = "sp0083", Name = "VITOP", Description = "Diệt...", Unit = null, StockQuantity = 50, LowStockThreshold = 10, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) }
            );
        }
    }
}
