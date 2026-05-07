using Microsoft.EntityFrameworkCore;
using agri_expert_service.Models;

namespace agri_expert_service.Data
{
    public class AgriDbContext : DbContext
    {
        public AgriDbContext(DbContextOptions<AgriDbContext> options) : base(options) { }

        public DbSet<ChemicalProfile> ChemicalProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChemicalProfile>(entity =>
            {
                entity.HasIndex(c => c.CasNumber).IsUnique().HasFilter("[CasNumber] IS NOT NULL");
                entity.HasIndex(c => c.Name);
            });

            // ── Seed 26 hóa chất ─────────────────────────────────────
            modelBuilder.Entity<ChemicalProfile>().HasData(
                new ChemicalProfile { Id = 1,  Name = "Boron",                 VietnameseName = "Bo",            CasNumber = "7440-42-8",    ChemicalGroup = "Vi lượng / Phân bón",          ToxicityLevel = "III",  Description = "Nguyên tố vi lượng cần thiết cho sự phát triển của thực vật.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 2,  Name = "Zinc Sulfate",          VietnameseName = "Kẽm sunfat",    CasNumber = "7733-02-0",    ChemicalGroup = "Vi lượng / Phân bón",          ToxicityLevel = "III",  Description = "Cung cấp kẽm cho cây trồng, khắc phục thiếu vi lượng.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 3,  Name = "Ethephon",              VietnameseName = "Ethephon",      CasNumber = "16672-87-0",   ChemicalGroup = "Điều hòa sinh trưởng",         ToxicityLevel = "II",   Description = "Chất điều hòa sinh trưởng kích thích chín trái, ra hoa.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 4,  Name = "Paclobutrazol",         VietnameseName = "Paclobutrazol", CasNumber = "76738-62-0",   ChemicalGroup = "Điều hòa sinh trưởng",         ToxicityLevel = "III",  Description = "Ức chế sinh trưởng chiều cao, kích thích ra hoa.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 5,  Name = "Fipronil",              VietnameseName = "Fipronil",      CasNumber = "120068-37-3",  ChemicalGroup = "Thuốc trừ sâu",                ToxicityLevel = "II",   Description = "Thuốc trừ sâu phổ rộng, tác dụng tiếp xúc và vị độc.", SafetyNotes = "Rất độc với cá và động vật thủy sinh.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 6,  Name = "Chlorpyrifos Methyl",   VietnameseName = "Clo-py-ri-phos methyl", CasNumber = "5598-13-0",  ChemicalGroup = "Thuốc trừ sâu",       ToxicityLevel = "II",   Description = "Nhóm organophosphate, trừ sâu tiếp xúc và vị độc.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 7,  Name = "Imidacloprid",          VietnameseName = "Imidacloprid",  CasNumber = "138261-41-3",  ChemicalGroup = "Thuốc trừ sâu",                ToxicityLevel = "II",   Description = "Neonicotinoid, tác động hệ thần kinh côn trùng, nội hấp.", SafetyNotes = "Nguy hiểm với ong mật.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 8,  Name = "Emamectin Benzoate",    VietnameseName = "Emamectin",     CasNumber = "155569-91-8",  ChemicalGroup = "Thuốc trừ sâu",                ToxicityLevel = "Ib",   Description = "Trừ sâu từ lên men vi sinh, đặc trị sâu cuốn lá, sâu đục thân.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 9,  Name = "Cartap Hydrochloride",  VietnameseName = "Cartap",        CasNumber = "15263-53-3",   ChemicalGroup = "Thuốc trừ sâu",                ToxicityLevel = "II",   Description = "Dẫn xuất nereistoxin, trừ sâu tiếp xúc và vị độc.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 10, Name = "Propargite",            VietnameseName = "Propargite",    CasNumber = "2312-35-8",    ChemicalGroup = "Thuốc trừ nhện",               ToxicityLevel = "II",   Description = "Trừ nhện đỏ và nhện trắng trên nhiều loại cây trồng.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 11, Name = "Tricyclazole",          VietnameseName = "Tricyclazole",  CasNumber = "41814-78-2",   ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Đặc trị bệnh đạo ôn trên lúa.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 12, Name = "Mancozeb",              VietnameseName = "Mancozeb",      CasNumber = "8018-01-7",    ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Thuốc diệt nấm phổ rộng nhóm dithiocarbamate.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 13, Name = "Hexaconazole",          VietnameseName = "Hexaconazole",  CasNumber = "79983-71-4",   ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Triazole, trừ nấm phổ rộng, nội hấp.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 14, Name = "Kasugamycin",           VietnameseName = "Kasugamycin",   CasNumber = "6980-18-3",    ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Kháng sinh nông nghiệp từ vi khuẩn, trừ bệnh đạo ôn và bạc lá lúa.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 15, Name = "Difenoconazole",        VietnameseName = "Difenoconazole",CasNumber = "119446-68-3",  ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "II",   Description = "Triazole nội hấp, trừ bệnh rỉ sắt, đốm lá phổ rộng.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 16, Name = "Bismerthiazol",         VietnameseName = "Bismerthiazol", CasNumber = "85038-74-8",   ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Trừ bệnh bạc lá lúa do vi khuẩn Xanthomonas.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 17, Name = "Copper Hydroxide",      VietnameseName = "Đồng hydroxide",CasNumber = "20427-59-2",   ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Thuốc trừ bệnh vô cơ, phổ rộng, tiếp xúc.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 18, Name = "Isoprothiolane",        VietnameseName = "Isoprothiolane",CasNumber = "50512-35-1",   ChemicalGroup = "Thuốc trừ bệnh",               ToxicityLevel = "III",  Description = "Trừ bệnh đạo ôn lúa, nội hấp mạnh.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 19, Name = "Glufosinate Ammonium",  VietnameseName = "Glufosinate",   CasNumber = "77182-82-2",   ChemicalGroup = "Thuốc diệt cỏ",                ToxicityLevel = "II",   Description = "Thuốc diệt cỏ không chọn lọc, tác động tiếp xúc.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 20, Name = "Atrazine",              VietnameseName = "Atrazine",      CasNumber = "1912-24-9",    ChemicalGroup = "Thuốc diệt cỏ",                ToxicityLevel = "III",  Description = "Thuốc diệt cỏ chọn lọc nhóm triazine, dùng cho ngô.", SafetyNotes = "Cần thận trọng với nguồn nước ngầm.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 21, Name = "Pretilachlor",          VietnameseName = "Pretilachlor",  CasNumber = "51218-49-6",   ChemicalGroup = "Thuốc diệt cỏ",                ToxicityLevel = "II",   Description = "Diệt cỏ chọn lọc lúa nước, dùng đầu vụ.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 22, Name = "Diuron",                VietnameseName = "Diuron",        CasNumber = "330-54-1",     ChemicalGroup = "Thuốc diệt cỏ",                ToxicityLevel = "III",  Description = "Thuốc diệt cỏ không chọn lọc, ức chế quang hợp.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 23, Name = "Niclosamide",           VietnameseName = "Niclosamide",   CasNumber = "50-65-7",      ChemicalGroup = "Thuốc diệt ốc bươu vàng",      ToxicityLevel = "II",   Description = "Diệt ốc bươu vàng trên ruộng lúa.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 24, Name = "Brodifacoum",           VietnameseName = "Brodifacoum",   CasNumber = "56073-10-0",   ChemicalGroup = "Thuốc diệt chuột",             ToxicityLevel = "Ia",   Description = "Thuốc diệt chuột chống đông máu thế hệ 2, hiệu quả cao.", SafetyNotes = "Cực độc — cần bảo quản xa tầm tay trẻ em và vật nuôi.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 25, Name = "Metaldehyde",           VietnameseName = "Metaldehyde",   CasNumber = "108-62-3",     ChemicalGroup = "Thuốc diệt ốc sên / nhớt",     ToxicityLevel = "II",   Description = "Diệt ốc sên, sên nhớt trên rau màu.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) },
                new ChemicalProfile { Id = 26, Name = "Annonin",               VietnameseName = "Annonin",       CasNumber = null,           ChemicalGroup = "Thuốc trừ sâu sinh học",       ToxicityLevel = "III",  Description = "Chiết xuất từ hạt na, trừ sâu sinh học thân thiện môi trường.", IsActive = true, CreatedAt = new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc) }
            );
        }
    }
}
