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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phân bón và hóa chất" },
                new Category { Id = 2, Name = "Thuốc trừ bệnh" },
                new Category { Id = 3, Name = "Thuốc trừ cỏ" },
                new Category { Id = 4, Name = "Thuốc trừ sâu" }
            );

            // Seed all 83 products from legacy data.txt with ChemicalProfileId mapping
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, SKU = "sp0001", Name = "Vi lượng-BOROZINC", Description = "Bo và Kẽm là các yếu tố vi lượng thiết yếu cho cây trồng...", UnitPrice = 180000m, CategoryId = 1, ChemicalProfileId = 1, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 2, SKU = "sp0002", Name = "TT-ANONIN 1EC", Description = "Thuốc trừ sâu sinh học, 100% nguồn gốc từ thực vật...", UnitPrice = 220000m, CategoryId = 1, ChemicalProfileId = 26, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 3, SKU = "sp0003", Name = "TT SNAILTA GOLD 750WP", Description = "Công thức mới với khả năng tác động mạnh...", UnitPrice = 220000m, CategoryId = 1, ChemicalProfileId = 23, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 4, SKU = "sp0004", Name = "TANO_606", Description = "Giúp ra hoa sớm, đồng loạt...", UnitPrice = 130000m, CategoryId = 1, ChemicalProfileId = 1, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 5, SKU = "sp0005", Name = "TANO_601", Description = "Giúp cây trồng phát triển nhanh, khỏe...", UnitPrice = 120000m, CategoryId = 1, ChemicalProfileId = null, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 6, SKU = "sp0006", Name = "SUNPHAT_KẼM", Description = "Sunphat Kẽm là muối Kẽm vô cơ...", UnitPrice = 220000m, CategoryId = 1, ChemicalProfileId = 2, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 7, SKU = "sp0007", Name = "SPC - NPK", Description = "Tăng độ cứng cây, chống chịu sâu bệnh...", UnitPrice = 200000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 8, SKU = "sp0008", Name = "TT BIOBECA 0.1SP", Description = "TT BIOBECA 0.1SP giúp giữ xanh lá đòng...", UnitPrice = 190000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 9, SKU = "sp0009", Name = "SPC_MKP", Description = "Kích thích phát triển bộ rễ...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 10, SKU = "sp0010", Name = "SPC_KALI_SILIC", Description = "Hạn chế chiều cao cây...", UnitPrice = 250000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 11, SKU = "sp0011", Name = "SAMINO_51SL", Description = "SAMINO 5.1 SL là chất kích thích sinh học...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 12, SKU = "sp0012", Name = "SAIGON_P115WP", Description = "SAIGON - P1 giúp tăng cường đẻ nhánh...", UnitPrice = 260000m, CategoryId = 1, ChemicalProfileId = 4, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 13, SKU = "sp0013", Name = "SAGOLATEX", Description = "SAGOLATEX 2.5 PA là thuốc kích thích...", UnitPrice = 190000m, CategoryId = 1, ChemicalProfileId = 3, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 14, SKU = "sp0014", Name = "SAGO SÓNG THẦN", Description = "Giúp thuốc BVTV loang trải đều...", UnitPrice = 195000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 15, SKU = "sp0015", Name = "SAGO ĐỒNG", Description = "SAGO ĐỒNG được sử dụng để pha chế...", UnitPrice = 250000m, CategoryId = 1, ChemicalProfileId = 17, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 16, SKU = "sp0016", Name = "SAGO SIÊU HẤP", Description = "Đang cập nhật", UnitPrice = 220000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 17, SKU = "sp0017", Name = "SAGO BÁM DÍNH", Description = "Tăng khả năng bám dính...", UnitPrice = 300000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 18, SKU = "sp0018", Name = "PLASTIMULA 1SL", Description = "Giúp phát triển bộ rễ khỏe...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 19, SKU = "sp0019", Name = "SAGO AXIT", Description = "Giúp phá vỡ trạng thái ngủ...", UnitPrice = 180000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 20, SKU = "sp0020", Name = "ORGANIC NOKAYO", Description = "Giúp đất tơi xốp, giàu mùn...", UnitPrice = 170000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 21, SKU = "sp0021", Name = "ORGANIC YUKIMOTO", Description = "Bổ sung hữu cơ cho đất bạc màu...", UnitPrice = 180000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 22, SKU = "sp0022", Name = "NPK HÀN VIỆT 20 20 15 TE", Description = "Cung cấp dinh dưỡng cân đối...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 23, SKU = "sp0023", Name = "NPK HÀN VIỆT 15-15-15", Description = "Thúc đẩy cây trồng phát triển đồng đều...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 24, SKU = "sp0024", Name = "NPK FERTIGONIA", Description = "FERTIGONIA là phân bón đa năng...", UnitPrice = 310000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 25, SKU = "sp0025", Name = "NATRAZYME", Description = "Natrazyme bổ sung đầy đủ và cân đối...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 26, SKU = "sp0026", Name = "NPK 16-16-8-13S", Description = "Cung cấp dinh dưỡng cân đối...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 27, SKU = "sp0027", Name = "SOP", Description = "Tăng sức chống chịu cho cây trồng...", UnitPrice = 240000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 28, SKU = "sp0028", Name = "NPK 17-7-17 + TE", Description = "Phân bón hỗn hợp NPK...", UnitPrice = 200000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 29, SKU = "sp0029", Name = "NPK 16-16-8 + TE", Description = "Phân NPK sản xuất bằng công nghệ...", UnitPrice = 190000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 30, SKU = "sp0030", Name = "NPK TÂN THÀNH 25-25-5", Description = "Cung cấp đồng thời Đạm – Lân – Kali...", UnitPrice = 310000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 31, SKU = "sp0031", Name = "NPK TÂN THÀNH 20-20-15 TE", Description = "Phân đa dinh dưỡng tổng hợp...", UnitPrice = 220000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 32, SKU = "sp0032", Name = "LACASOTO 4SP", Description = "Thuốc sinh học giúp tăng năng suất lúa...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 33, SKU = "sp0033", Name = "CHUBECA", Description = "Polyphenol giúp cây trồng...", UnitPrice = 170000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 34, SKU = "sp0034", Name = "DAP TÂN THÀNH", Description = "Cung cấp dinh dưỡng thiết yếu...", UnitPrice = 160000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 35, SKU = "sp0035", Name = "ATRAZIN", Description = "Cải tạo đất, tăng độ tơi xốp...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 36, SKU = "sp0036", Name = "KẼM BORON", Description = "Kẽm và Boron có vai trò thiết yếu...", UnitPrice = 200000m, CategoryId = 1, ChemicalProfileId = 1, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 37, SKU = "sp0037", Name = "ANIMAT", Description = "Bo và Kẽm có vai trò thiết yếu...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = 1, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 38, SKU = "sp0038", Name = "SPC - MKP", Description = "Giúp kích thích phát triển bộ rễ...", UnitPrice = 210000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 39, SKU = "sp0039", Name = "CHOCASO", Description = "Chocaso là thuốc tăng trưởng sinh học...", UnitPrice = 300000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 40, SKU = "sp0040", Name = "NKP TE", Description = "Phân NPK sản xuất theo công nghệ...", UnitPrice = 230000m, CategoryId = 1, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 41, SKU = "sp0041", Name = "ZINEB BUL 80WP", Description = "Thuốc trừ nấm tiếp xúc...", UnitPrice = 170000m, CategoryId = 2, ChemicalProfileId = 12, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 42, SKU = "sp0042", Name = "ZIN 80 WP", Description = "Thuốc trừ nấm tác dụng tiếp xúc...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = 12, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 43, SKU = "sp0043", Name = "XINAZO", Description = "XINAZO 250 SC là thuốc trừ nấm...", UnitPrice = 200000m, CategoryId = 2, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 44, SKU = "sp0044", Name = "VANICIDE", Description = "VANICIDE là thuốc trừ bệnh sinh học...", UnitPrice = 175000m, CategoryId = 2, ChemicalProfileId = 14, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 45, SKU = "sp0045", Name = "UNITIL", Description = "Thuốc có tác dụng nội hấp mạnh...", UnitPrice = 260000m, CategoryId = 2, ChemicalProfileId = 14, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 46, SKU = "sp0046", Name = "BASU 250WP", Description = "Thuốc đặc trị vi khuẩn...", UnitPrice = 150000m, CategoryId = 2, ChemicalProfileId = 16, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 47, SKU = "sp0047", Name = "KEEP 300SC", Description = "Công thức mới đặc trị bệnh đạo ôn...", UnitPrice = 175000m, CategoryId = 1, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 48, SKU = "sp0048", Name = "ATANIL", Description = "Thuốc đặc trị bệnh cháy bìa lá...", UnitPrice = 230000m, CategoryId = 2, ChemicalProfileId = 16, IsActive = false, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 49, SKU = "sp0049", Name = "BIOMYCIN", Description = "Thuốc có tác dụng tiếp xúc và nội hấp...", UnitPrice = 185000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 50, SKU = "sp0050", Name = "Regent", Description = "Thuốc trừ sâu Regent là sản phẩm đặc trị...", UnitPrice = 199000m, CategoryId = 4, ChemicalProfileId = 5, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 51, SKU = "sp0051", Name = "TRIZOLE 400SC", Description = "Trizole 400 SC là thuốc trừ nấm...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 52, SKU = "sp0052", Name = "TRIZOLE 75WP", Description = "Trizole 75WP là thuốc trừ nấm...", UnitPrice = 260000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 53, SKU = "sp0053", Name = "TRIZOLE 75 DO", Description = "Trizole 75 DO là thuốc trừ nấm...", UnitPrice = 240000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 54, SKU = "sp0054", Name = "TRI 75WG", Description = "TRI 75WG là thuốc trừ bệnh lưu dẫn...", UnitPrice = 240000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 55, SKU = "sp0055", Name = "TREPPACH BUL", Description = "TREPPACH BUL là thuốc trừ nấm...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 56, SKU = "sp0056", Name = "SAIPORA", Description = "SAIPORA là thuốc trừ nấm...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 57, SKU = "sp0057", Name = "SAGOPERFECT 320", Description = "SAGOPERFECT 320 là thuốc trừ nấm...", UnitPrice = 270000m, CategoryId = 2, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 58, SKU = "sp0058", Name = "SAGOGRAIN 300EC", Description = "Diệt mầm bệnh nhanh chóng...", UnitPrice = 310000m, CategoryId = 2, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 59, SKU = "sp0059", Name = "ROTEVA 30SC", Description = "ROTEVA 30SC giúp trị bệnh...", UnitPrice = 150000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 60, SKU = "sp0060", Name = "PYROLAX 250EC", Description = "Thuốc có tác dụng phòng và trị bệnh...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 61, SKU = "sp0061", Name = "OTICIN 47.5WP", Description = "Thuốc phòng trị hiệu quả...", UnitPrice = 240000m, CategoryId = 2, ChemicalProfileId = 17, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 62, SKU = "sp0062", Name = "MEXYL MZ", Description = "Thuốc phòng trị hiệu quả...", UnitPrice = 140000m, CategoryId = 2, ChemicalProfileId = 12, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 63, SKU = "sp0063", Name = "LUSTER 250SC", Description = "Thuốc có tác dụng phòng trị...", UnitPrice = 250000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 64, SKU = "sp0064", Name = "LUNASA 80", Description = "Giúp diệt nấm bệnh nhanh chóng...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = 15, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 65, SKU = "sp0065", Name = "LÚA VÀNG", Description = "Thuốc có hiệu quả cao...", UnitPrice = 260000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 66, SKU = "sp0066", Name = "KAISAIGON", Description = "Thuốc trừ nấm nội hấp...", UnitPrice = 230000m, CategoryId = 2, ChemicalProfileId = 18, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 67, SKU = "sp0067", Name = "KAISAIGON 10", Description = "Thuốc đặc trị các bệnh nấm...", UnitPrice = 150000m, CategoryId = 2, ChemicalProfileId = 18, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 68, SKU = "sp0068", Name = "CỐP XANH", Description = "Thuốc phòng trị hiệu quả...", UnitPrice = 250000m, CategoryId = 2, ChemicalProfileId = 17, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 69, SKU = "sp0069", Name = "HỎA TIỄN", Description = "HỎA TIỄN 50 SP là thuốc trừ bệnh...", UnitPrice = 250000m, CategoryId = 2, ChemicalProfileId = 16, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 70, SKU = "sp0070", Name = "HẠT VÀNG", Description = "HẠT VÀNG có khả năng phòng và trị...", UnitPrice = 215000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 71, SKU = "sp0071", Name = "EDIVIL 80WP", Description = "Edivil 80WP là sự kết hợp...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = 11, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 72, SKU = "sp0072", Name = "DIPOMATE", Description = "DIPOMATE 430 SC là thuốc trừ nấm...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = 12, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 73, SKU = "sp0073", Name = "DIPOMATE 80", Description = "Phòng trị hiệu quả nhiều bệnh...", UnitPrice = 130000m, CategoryId = 2, ChemicalProfileId = 12, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 74, SKU = "sp0074", Name = "CLEARNER 75 WP", Description = "CLEARNER 75 WP là thuốc trừ nấm...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 75, SKU = "sp0075", Name = "CHUBECA 1.8SL", Description = "Đặc trị lem lép hạt trên lúa...", UnitPrice = 130000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 76, SKU = "sp0076", Name = "ALPINE 80WP", Description = "ALPINE có tác dụng lưu dẫn...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 77, SKU = "sp0077", Name = "ALPINE XANH", Description = "ALPINE 80 WG là thuốc trừ nấm...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 78, SKU = "sp0078", Name = "SULOX 80WP", Description = "Đặc trị bệnh phấn trắng...", UnitPrice = 210000m, CategoryId = 2, ChemicalProfileId = null, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 79, SKU = "sp0079", Name = "SAIZOLE 5SC", Description = "Thuốc trừ nấm có khả năng nội hấp...", UnitPrice = 310000m, CategoryId = 2, ChemicalProfileId = 13, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 80, SKU = "sp0080", Name = "SAIPAN 2SL", Description = "Thuốc trừ nấm và vi khuẩn...", UnitPrice = 220000m, CategoryId = 2, ChemicalProfileId = 14, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 81, SKU = "sp0081", Name = "YOSKY", Description = "Thuốc trừ cỏ không chọn lọc...", UnitPrice = 310000m, CategoryId = 3, ChemicalProfileId = 19, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 82, SKU = "sp0082", Name = "XINRON", Description = "Thuốc được dùng để diệt cỏ...", UnitPrice = 250000m, CategoryId = 3, ChemicalProfileId = 22, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new Product { Id = 83, SKU = "sp0083", Name = "VITOP", Description = "Diệt hầu hết các loại cỏ dại...", UnitPrice = 310000m, CategoryId = 3, ChemicalProfileId = 21, IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) }
            );

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
