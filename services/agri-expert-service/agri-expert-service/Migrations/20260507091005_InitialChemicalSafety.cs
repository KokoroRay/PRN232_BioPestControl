using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace agri_expert_service.Migrations
{
    /// <inheritdoc />
    public partial class InitialChemicalSafety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChemicalProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VietnameseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CasNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChemicalGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChemicalFormula = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ToxicityLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UsageMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SafetyNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetCrops = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetPests = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ChemicalProfiles",
                columns: new[] { "Id", "CasNumber", "ChemicalFormula", "ChemicalGroup", "CreatedAt", "Description", "IsActive", "Name", "SafetyNotes", "TargetCrops", "TargetPests", "ToxicityLevel", "UpdatedAt", "UsageMethod", "VietnameseName" },
                values: new object[,]
                {
                    { 1, "7440-42-8", null, "Vi lượng / Phân bón", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nguyên tố vi lượng cần thiết cho sự phát triển của thực vật.", true, "Boron", null, null, null, "III", null, null, "Bo" },
                    { 2, "7733-02-0", null, "Vi lượng / Phân bón", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cung cấp kẽm cho cây trồng, khắc phục thiếu vi lượng.", true, "Zinc Sulfate", null, null, null, "III", null, null, "Kẽm sunfat" },
                    { 3, "16672-87-0", null, "Điều hòa sinh trưởng", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chất điều hòa sinh trưởng kích thích chín trái, ra hoa.", true, "Ethephon", null, null, null, "II", null, null, "Ethephon" },
                    { 4, "76738-62-0", null, "Điều hòa sinh trưởng", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ức chế sinh trưởng chiều cao, kích thích ra hoa.", true, "Paclobutrazol", null, null, null, "III", null, null, "Paclobutrazol" },
                    { 5, "120068-37-3", null, "Thuốc trừ sâu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc trừ sâu phổ rộng, tác dụng tiếp xúc và vị độc.", true, "Fipronil", "Rất độc với cá và động vật thủy sinh.", null, null, "II", null, null, "Fipronil" },
                    { 6, "5598-13-0", null, "Thuốc trừ sâu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nhóm organophosphate, trừ sâu tiếp xúc và vị độc.", true, "Chlorpyrifos Methyl", null, null, null, "II", null, null, "Clo-py-ri-phos methyl" },
                    { 7, "138261-41-3", null, "Thuốc trừ sâu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Neonicotinoid, tác động hệ thần kinh côn trùng, nội hấp.", true, "Imidacloprid", "Nguy hiểm với ong mật.", null, null, "II", null, null, "Imidacloprid" },
                    { 8, "155569-91-8", null, "Thuốc trừ sâu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trừ sâu từ lên men vi sinh, đặc trị sâu cuốn lá, sâu đục thân.", true, "Emamectin Benzoate", null, null, null, "Ib", null, null, "Emamectin" },
                    { 9, "15263-53-3", null, "Thuốc trừ sâu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dẫn xuất nereistoxin, trừ sâu tiếp xúc và vị độc.", true, "Cartap Hydrochloride", null, null, null, "II", null, null, "Cartap" },
                    { 10, "2312-35-8", null, "Thuốc trừ nhện", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trừ nhện đỏ và nhện trắng trên nhiều loại cây trồng.", true, "Propargite", null, null, null, "II", null, null, "Propargite" },
                    { 11, "41814-78-2", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Đặc trị bệnh đạo ôn trên lúa.", true, "Tricyclazole", null, null, null, "III", null, null, "Tricyclazole" },
                    { 12, "8018-01-7", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc diệt nấm phổ rộng nhóm dithiocarbamate.", true, "Mancozeb", null, null, null, "III", null, null, "Mancozeb" },
                    { 13, "79983-71-4", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Triazole, trừ nấm phổ rộng, nội hấp.", true, "Hexaconazole", null, null, null, "III", null, null, "Hexaconazole" },
                    { 14, "6980-18-3", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kháng sinh nông nghiệp từ vi khuẩn, trừ bệnh đạo ôn và bạc lá lúa.", true, "Kasugamycin", null, null, null, "III", null, null, "Kasugamycin" },
                    { 15, "119446-68-3", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Triazole nội hấp, trừ bệnh rỉ sắt, đốm lá phổ rộng.", true, "Difenoconazole", null, null, null, "II", null, null, "Difenoconazole" },
                    { 16, "85038-74-8", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trừ bệnh bạc lá lúa do vi khuẩn Xanthomonas.", true, "Bismerthiazol", null, null, null, "III", null, null, "Bismerthiazol" },
                    { 17, "20427-59-2", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc trừ bệnh vô cơ, phổ rộng, tiếp xúc.", true, "Copper Hydroxide", null, null, null, "III", null, null, "Đồng hydroxide" },
                    { 18, "50512-35-1", null, "Thuốc trừ bệnh", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trừ bệnh đạo ôn lúa, nội hấp mạnh.", true, "Isoprothiolane", null, null, null, "III", null, null, "Isoprothiolane" },
                    { 19, "77182-82-2", null, "Thuốc diệt cỏ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc diệt cỏ không chọn lọc, tác động tiếp xúc.", true, "Glufosinate Ammonium", null, null, null, "II", null, null, "Glufosinate" },
                    { 20, "1912-24-9", null, "Thuốc diệt cỏ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc diệt cỏ chọn lọc nhóm triazine, dùng cho ngô.", true, "Atrazine", "Cần thận trọng với nguồn nước ngầm.", null, null, "III", null, null, "Atrazine" },
                    { 21, "51218-49-6", null, "Thuốc diệt cỏ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diệt cỏ chọn lọc lúa nước, dùng đầu vụ.", true, "Pretilachlor", null, null, null, "II", null, null, "Pretilachlor" },
                    { 22, "330-54-1", null, "Thuốc diệt cỏ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc diệt cỏ không chọn lọc, ức chế quang hợp.", true, "Diuron", null, null, null, "III", null, null, "Diuron" },
                    { 23, "50-65-7", null, "Thuốc diệt ốc bươu vàng", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diệt ốc bươu vàng trên ruộng lúa.", true, "Niclosamide", null, null, null, "II", null, null, "Niclosamide" },
                    { 24, "56073-10-0", null, "Thuốc diệt chuột", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thuốc diệt chuột chống đông máu thế hệ 2, hiệu quả cao.", true, "Brodifacoum", "Cực độc — cần bảo quản xa tầm tay trẻ em và vật nuôi.", null, null, "Ia", null, null, "Brodifacoum" },
                    { 25, "108-62-3", null, "Thuốc diệt ốc sên / nhớt", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diệt ốc sên, sên nhớt trên rau màu.", true, "Metaldehyde", null, null, null, "II", null, null, "Metaldehyde" },
                    { 26, null, null, "Thuốc trừ sâu sinh học", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chiết xuất từ hạt na, trừ sâu sinh học thân thiện môi trường.", true, "Annonin", null, null, null, "III", null, null, "Annonin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalProfiles_CasNumber",
                table: "ChemicalProfiles",
                column: "CasNumber",
                unique: true,
                filter: "[CasNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalProfiles_Name",
                table: "ChemicalProfiles",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChemicalProfiles");
        }
    }
}
