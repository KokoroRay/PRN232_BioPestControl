using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace catalog_service.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductsCatalogUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ChemicalProfileId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ChemicalProfileId",
                value: 26);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { 23, "Công thức mới với khả năng tác động mạnh..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { 1, "Giúp ra hoa sớm, đồng loạt..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ChemicalProfileId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "Kích thích phát triển bộ rễ...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "Hạn chế chiều cao cây...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "SAMINO 5.1 SL là chất kích thích sinh học...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                column: "ChemicalProfileId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { 3, "SAGOLATEX 2.5 PA là thuốc kích thích..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                column: "Description",
                value: "Giúp thuốc BVTV loang trải đều...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { 17, "SAGO ĐỒNG được sử dụng để pha chế..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                column: "Description",
                value: "Tăng khả năng bám dính...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "Description",
                value: "Giúp phát triển bộ rễ khỏe...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                column: "Description",
                value: "Giúp phá vỡ trạng thái ngủ...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                column: "Description",
                value: "Giúp đất tơi xốp, giàu mùn...");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "ChemicalProfileId", "CreatedAt", "CreatedByAdminId", "Description", "ImageUrl", "IsActive", "ManagedByStaffId", "Name", "SKU", "Unit", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { 21, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bổ sung hữu cơ cho đất bạc màu...", null, true, null, "ORGANIC YUKIMOTO", "sp0021", null, 180000m, null },
                    { 22, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cung cấp dinh dưỡng cân đối...", null, true, null, "NPK HÀN VIỆT 20 20 15 TE", "sp0022", null, 210000m, null },
                    { 23, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thúc đẩy cây trồng phát triển đồng đều...", null, true, null, "NPK HÀN VIỆT 15-15-15", "sp0023", null, 210000m, null },
                    { 24, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "FERTIGONIA là phân bón đa năng...", null, true, null, "NPK FERTIGONIA", "sp0024", null, 310000m, null },
                    { 25, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Natrazyme bổ sung đầy đủ và cân đối...", null, true, null, "NATRAZYME", "sp0025", null, 230000m, null },
                    { 26, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cung cấp dinh dưỡng cân đối...", null, true, null, "NPK 16-16-8-13S", "sp0026", null, 210000m, null },
                    { 27, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tăng sức chống chịu cho cây trồng...", null, true, null, "SOP", "sp0027", null, 240000m, null },
                    { 28, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phân bón hỗn hợp NPK...", null, true, null, "NPK 17-7-17 + TE", "sp0028", null, 200000m, null },
                    { 29, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phân NPK sản xuất bằng công nghệ...", null, true, null, "NPK 16-16-8 + TE", "sp0029", null, 190000m, null },
                    { 30, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cung cấp đồng thời Đạm – Lân – Kali...", null, true, null, "NPK TÂN THÀNH 25-25-5", "sp0030", null, 310000m, null },
                    { 31, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phân đa dinh dưỡng tổng hợp...", null, true, null, "NPK TÂN THÀNH 20-20-15 TE", "sp0031", null, 220000m, null },
                    { 32, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc sinh học giúp tăng năng suất lúa...", null, true, null, "LACASOTO 4SP", "sp0032", null, 210000m, null },
                    { 33, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Polyphenol giúp cây trồng...", null, true, null, "CHUBECA", "sp0033", null, 170000m, null },
                    { 34, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cung cấp dinh dưỡng thiết yếu...", null, true, null, "DAP TÂN THÀNH", "sp0034", null, 160000m, null },
                    { 35, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cải tạo đất, tăng độ tơi xốp...", null, true, null, "ATRAZIN", "sp0035", null, 230000m, null },
                    { 36, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kẽm và Boron có vai trò thiết yếu...", null, true, null, "KẼM BORON", "sp0036", null, 200000m, null },
                    { 37, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bo và Kẽm có vai trò thiết yếu...", null, true, null, "ANIMAT", "sp0037", null, 230000m, null },
                    { 38, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Giúp kích thích phát triển bộ rễ...", null, true, null, "SPC - MKP", "sp0038", null, 210000m, null },
                    { 39, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chocaso là thuốc tăng trưởng sinh học...", null, true, null, "CHOCASO", "sp0039", null, 300000m, null },
                    { 40, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phân NPK sản xuất theo công nghệ...", null, true, null, "NKP TE", "sp0040", null, 230000m, null },
                    { 41, 2, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ nấm tiếp xúc...", null, true, null, "ZINEB BUL 80WP", "sp0041", null, 170000m, null },
                    { 42, 2, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ nấm tác dụng tiếp xúc...", null, true, null, "ZIN 80 WP", "sp0042", null, 210000m, null },
                    { 43, 2, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "XINAZO 250 SC là thuốc trừ nấm...", null, true, null, "XINAZO", "sp0043", null, 200000m, null },
                    { 44, 2, 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "VANICIDE là thuốc trừ bệnh sinh học...", null, true, null, "VANICIDE", "sp0044", null, 175000m, null },
                    { 45, 2, 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc có tác dụng nội hấp mạnh...", null, true, null, "UNITIL", "sp0045", null, 260000m, null },
                    { 46, 2, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc đặc trị vi khuẩn...", null, true, null, "BASU 250WP", "sp0046", null, 150000m, null },
                    { 47, 1, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Công thức mới đặc trị bệnh đạo ôn...", null, true, null, "KEEP 300SC", "sp0047", null, 175000m, null },
                    { 48, 2, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc đặc trị bệnh cháy bìa lá...", null, false, null, "ATANIL", "sp0048", null, 230000m, null },
                    { 49, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc có tác dụng tiếp xúc và nội hấp...", null, true, null, "BIOMYCIN", "sp0049", null, 185000m, null },
                    { 50, 4, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ sâu Regent là sản phẩm đặc trị...", null, true, null, "Regent", "sp0050", null, 199000m, null },
                    { 51, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trizole 400 SC là thuốc trừ nấm...", null, true, null, "TRIZOLE 400SC", "sp0051", null, 210000m, null },
                    { 52, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trizole 75WP là thuốc trừ nấm...", null, true, null, "TRIZOLE 75WP", "sp0052", null, 260000m, null },
                    { 53, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trizole 75 DO là thuốc trừ nấm...", null, true, null, "TRIZOLE 75 DO", "sp0053", null, 240000m, null },
                    { 54, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "TRI 75WG là thuốc trừ bệnh lưu dẫn...", null, true, null, "TRI 75WG", "sp0054", null, 240000m, null },
                    { 55, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "TREPPACH BUL là thuốc trừ nấm...", null, true, null, "TREPPACH BUL", "sp0055", null, 220000m, null },
                    { 56, 2, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "SAIPORA là thuốc trừ nấm...", null, true, null, "SAIPORA", "sp0056", null, 210000m, null },
                    { 57, 2, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "SAGOPERFECT 320 là thuốc trừ nấm...", null, true, null, "SAGOPERFECT 320", "sp0057", null, 270000m, null },
                    { 58, 2, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Diệt mầm bệnh nhanh chóng...", null, true, null, "SAGOGRAIN 300EC", "sp0058", null, 310000m, null },
                    { 59, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ROTEVA 30SC giúp trị bệnh...", null, true, null, "ROTEVA 30SC", "sp0059", null, 150000m, null },
                    { 60, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc có tác dụng phòng và trị bệnh...", null, true, null, "PYROLAX 250EC", "sp0060", null, 210000m, null },
                    { 61, 2, 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc phòng trị hiệu quả...", null, true, null, "OTICIN 47.5WP", "sp0061", null, 240000m, null },
                    { 62, 2, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc phòng trị hiệu quả...", null, true, null, "MEXYL MZ", "sp0062", null, 140000m, null },
                    { 63, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc có tác dụng phòng trị...", null, true, null, "LUSTER 250SC", "sp0063", null, 250000m, null },
                    { 64, 2, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Giúp diệt nấm bệnh nhanh chóng...", null, true, null, "LUNASA 80", "sp0064", null, 220000m, null },
                    { 65, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc có hiệu quả cao...", null, true, null, "LÚA VÀNG", "sp0065", null, 260000m, null },
                    { 66, 2, 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ nấm nội hấp...", null, true, null, "KAISAIGON", "sp0066", null, 230000m, null },
                    { 67, 2, 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc đặc trị các bệnh nấm...", null, true, null, "KAISAIGON 10", "sp0067", null, 150000m, null },
                    { 68, 2, 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc phòng trị hiệu quả...", null, true, null, "CỐP XANH", "sp0068", null, 250000m, null },
                    { 69, 2, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "HỎA TIỄN 50 SP là thuốc trừ bệnh...", null, true, null, "HỎA TIỄN", "sp0069", null, 250000m, null },
                    { 70, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "HẠT VÀNG có khả năng phòng và trị...", null, true, null, "HẠT VÀNG", "sp0070", null, 215000m, null },
                    { 71, 2, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Edivil 80WP là sự kết hợp...", null, true, null, "EDIVIL 80WP", "sp0071", null, 210000m, null },
                    { 72, 2, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "DIPOMATE 430 SC là thuốc trừ nấm...", null, true, null, "DIPOMATE", "sp0072", null, 220000m, null },
                    { 73, 2, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phòng trị hiệu quả nhiều bệnh...", null, true, null, "DIPOMATE 80", "sp0073", null, 130000m, null },
                    { 74, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "CLEARNER 75 WP là thuốc trừ nấm...", null, true, null, "CLEARNER 75 WP", "sp0074", null, 210000m, null },
                    { 75, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đặc trị lem lép hạt trên lúa...", null, true, null, "CHUBECA 1.8SL", "sp0075", null, 130000m, null },
                    { 76, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ALPINE có tác dụng lưu dẫn...", null, true, null, "ALPINE 80WP", "sp0076", null, 220000m, null },
                    { 77, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ALPINE 80 WG là thuốc trừ nấm...", null, true, null, "ALPINE XANH", "sp0077", null, 220000m, null },
                    { 78, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đặc trị bệnh phấn trắng...", null, true, null, "SULOX 80WP", "sp0078", null, 210000m, null },
                    { 79, 2, 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ nấm có khả năng nội hấp...", null, true, null, "SAIZOLE 5SC", "sp0079", null, 310000m, null },
                    { 80, 2, 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ nấm và vi khuẩn...", null, true, null, "SAIPAN 2SL", "sp0080", null, 220000m, null },
                    { 81, 3, 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc trừ cỏ không chọn lọc...", null, true, null, "YOSKY", "sp0081", null, 310000m, null },
                    { 82, 3, 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thuốc được dùng để diệt cỏ...", null, true, null, "XINRON", "sp0082", null, 250000m, null },
                    { 83, 3, 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Diệt hầu hết các loại cỏ dại...", null, true, null, "VITOP", "sp0083", null, 310000m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ChemicalProfileId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ChemicalProfileId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { null, "Công thức mới với khả năng tác động mạnh lên hệ hô hấp..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { null, "Giúp ra hoa sớm, đồng loạt. Tăng tỷ lệ thụ phấn..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ChemicalProfileId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "Kích thích phát triển bộ rễ và giúp cây trồng phát triển khỏe mạnh...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "Hạn chế chiều cao cây, làm lá dày và dẹt ngắn...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "SAMINO 5.1 SL là chất kích thích sinh học thực vật...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                column: "ChemicalProfileId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { null, "SAGOLATEX 2.5 PA là thuốc kích thích sinh trưởng..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                column: "Description",
                value: "Giúp thuốc BVTV loang trải đều trên bề mặt tiếp xúc...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ChemicalProfileId", "Description" },
                values: new object[] { null, "SAGO ĐỒNG được sử dụng trong nông nghiệp để pha chế dung dịch Bordeaux..." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                column: "Description",
                value: "Tăng khả năng bám dính và loang trải của phân bón lá...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "Description",
                value: "Giúp phát triển bộ rễ khỏe, tăng chồi hữu hiệu...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                column: "Description",
                value: "Giúp phá vỡ trạng thái ngủ cho lúa giống...");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                column: "Description",
                value: "Giúp đất tơi xốp, giàu mùn, giữ ẩm tốt...");
        }
    }
}
