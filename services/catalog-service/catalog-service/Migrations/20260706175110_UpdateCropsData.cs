using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace catalog_service.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCropsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 8 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 9 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 22 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 24 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 47 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 50 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 60 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 78 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 81 });

            migrationBuilder.InsertData(
                table: "Crops",
                columns: new[] { "Id", "Description", "ImageUrl", "IsActive", "Name", "Slug" },
                values: new object[,]
                {
                    { 7, "Cây công nghiệp dài ngày, rất dễ nhiễm rệp sáp, tuyến trùng, bệnh gỉ sắt và nấm hồng.", "https://images.unsplash.com/photo-1511920170033-f8396924c348?q=80&w=600&auto=format&fit=crop", true, "Cà Phê", "ca-phe" },
                    { 8, "Cây ăn trái có giá trị cao, thường mắc bệnh nứt thân xì mủ do Phytophthora, rầy phấn, nhện đỏ.", "https://res.cloudinary.com/biopestcontrol/image/upload/v1700000001/durian.jpg", true, "Sầu Riêng", "sau-rieng" },
                    { 9, "Cây công nghiệp dễ bị bệnh chết nhanh, chết chậm do nấm Phytophthora và tuyến trùng rễ.", "https://res.cloudinary.com/biopestcontrol/image/upload/v1700000002/pepper.jpg", true, "Hồ Tiêu", "ho-tieu" },
                    { 10, "Thường bị bệnh thán thư hại bông và trái, rầy xanh, rệp sáp, cần phun thuốc định kỳ lúc ra hoa.", "https://images.unsplash.com/photo-1553284965-83fd3e82fa5a?q=80&w=600&auto=format&fit=crop", true, "Xoài", "xoai" },
                    { 11, "Rau ăn lá rất dễ bị sâu tơ, sâu xanh bướm trắng, bệnh thối nhũn vi khuẩn.", "https://images.unsplash.com/photo-1518972554746-b31c195f19e4?q=80&w=600&auto=format&fit=crop", true, "Bắp Cải", "bap-cai" },
                    { 12, "Nhóm cây có múi, thường bị sâu vẽ bùa, rầy chổng cánh, nhện đỏ và bệnh vàng lá gân xanh.", "https://images.unsplash.com/photo-1558293674-1e0e854497e6?q=80&w=600&auto=format&fit=crop", true, "Cam, Bưởi", "cam-buoi" },
                    { 13, "Cây lương thực ngắn ngày, thường bị sâu keo mùa thu, rệp cờ, bệnh khô vằn.", "https://images.unsplash.com/photo-1551754655-cd27e38d2076?q=80&w=600&auto=format&fit=crop", true, "Ngô (Bắp)", "ngo" }
                });

            migrationBuilder.UpdateData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 1 },
                column: "UsageInstruction",
                value: "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái.");

            migrationBuilder.InsertData(
                table: "ProductCrops",
                columns: new[] { "CropId", "ProductId", "UsageInstruction" },
                values: new object[,]
                {
                    { 3, 1, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 3, 3, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 4, 4, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 4, 5, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 5, 5, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 1, 6, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 2, 7, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 6, 7, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 1, 9, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 1, 11, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 3, 11, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 1, 13, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 1, 14, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 2, 14, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 5, 14, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 2, 16, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 4, 16, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 2, 17, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 4, 17, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 1, 19, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 6, 19, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 1, 20, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 3, 20, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 1, 21, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 4, 21, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 1, 22, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 1, 23, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 1, 24, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 5, 25, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 6, 25, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 5, 26, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 6, 27, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 4, 28, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 6, 28, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 1, 29, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 5, 30, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 2, 31, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 5, 32, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 2, 33, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 4, 36, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 1, 37, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 5, 37, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 1, 38, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 3, 38, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 4, 38, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 3, 39, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 2, 40, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 6, 40, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 2, 41, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 5, 41, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 1, 42, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 4, 44, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 3, 45, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 4, 45, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 1, 46, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 2, 48, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 4, 49, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 6, 52, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 2, 55, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 3, 56, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 6, 57, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 2, 58, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 3, 59, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 5, 59, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 4, 61, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 5, 61, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 4, 62, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 3, 63, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 4, 63, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 2, 65, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 6, 65, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 2, 70, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 5, 71, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 3, 73, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 1, 74, "Phòng trừ sâu bệnh, giúp cứng cây, đứng lá, tăng năng suất lúa." },
                    { 5, 75, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 4, 76, "Kiểm soát rệp sáp, nấm hồng trên cây công nghiệp." },
                    { 5, 78, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 6, 78, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 5, 79, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 3, 80, "An toàn cho rau màu, phòng trừ sâu tơ, sâu xanh." },
                    { 5, 80, "Trừ tuyến trùng, bảo vệ củ, giúp củ to, đều." },
                    { 6, 81, "Giữ hoa bền màu, phòng bệnh đốm lá." },
                    { 2, 82, "Bổ sung dinh dưỡng và bảo vệ nấm bệnh cho cây ăn trái." },
                    { 7, 1, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 10, 2, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 10, 3, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 11, 6, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 10, 8, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 8, 9, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 9, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 12, 10, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 7, 11, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 8, 12, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 7, 13, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 11, 13, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 11, 15, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 9, 17, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 8, 18, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 9, 20, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 7, 22, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 11, 22, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 7, 23, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 9, 24, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 12, 24, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 9, 25, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 7, 28, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 8, 29, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 29, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 10, 30, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 13, 30, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 11, 32, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 8, 34, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 35, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 13, 35, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 8, 36, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 36, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 9, 37, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 10, 41, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 11, 42, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 12, 42, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 13, 43, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 8, 44, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 9, 44, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 13, 47, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 11, 48, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 12, 48, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 8, 50, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 50, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 8, 51, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 12, 52, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 7, 53, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 13, 53, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 11, 54, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 13, 54, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 8, 57, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 10, 57, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 12, 58, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 12, 59, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 9, 60, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 13, 60, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 10, 61, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 11, 62, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 9, 63, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 11, 64, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 11, 66, "Kiểm soát sâu bệnh trên bắp cải hiệu quả." },
                    { 12, 66, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 13, 66, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 7, 67, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 12, 67, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 13, 67, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 8, 68, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 9, 69, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 10, 70, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 7, 71, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 10, 71, "Bảo vệ hoa và trái non xoài khỏi thán thư." },
                    { 7, 72, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 8, 73, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 13, 73, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 12, 77, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 9, 79, "Bảo vệ rễ hồ tiêu, ngừa bệnh chết nhanh chết chậm." },
                    { 13, 79, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 13, 80, "Ngừa sâu keo mùa thu, bệnh khô vằn trên ngô." },
                    { 8, 81, "Kiểm soát nứt thân xì mủ, rầy phấn trắng trên sầu riêng." },
                    { 7, 82, "Phòng trừ rỉ sắt, rệp sáp hại cà phê." },
                    { 12, 82, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." },
                    { 12, 83, "Trừ nhện đỏ, rầy chổng cánh bảo vệ cam bưởi." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 6 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 7 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 8 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 9 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 9 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 10 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 11 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 11 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 11 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 12 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 13 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 13 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 13 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 14 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 14 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 14 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 15 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 16 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 16 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 17 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 17 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 17 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 18 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 19 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 19 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 20 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 20 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 20 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 21 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 21 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 22 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 22 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 22 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 23 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 23 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 24 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 24 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 24 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 25 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 25 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 25 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 26 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 27 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 28 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 28 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 28 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 29 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 29 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 29 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 30 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 30 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 30 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 31 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 32 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 32 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 33 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 34 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 35 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 35 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 36 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 36 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 36 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 37 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 37 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 37 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 38 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 38 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 38 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 39 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 40 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 40 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 41 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 41 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 41 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 42 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 42 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 42 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 43 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 44 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 44 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 44 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 45 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 45 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 46 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 47 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 48 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 48 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 48 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 49 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 50 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 50 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 51 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 52 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 52 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 53 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 53 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 54 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 54 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 55 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 56 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 57 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 57 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 57 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 58 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 58 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 59 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 59 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 59 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 60 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 60 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 61 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 61 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 61 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 62 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 62 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 63 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 63 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 63 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 64 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 65 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 65 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 11, 66 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 66 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 66 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 67 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 67 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 67 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 68 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 69 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 70 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 70 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 71 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 71 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 10, 71 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 72 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 73 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 73 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 73 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 1, 74 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 75 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 4, 76 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 77 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 78 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 78 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 79 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 9, 79 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 79 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 3, 80 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 5, 80 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 13, 80 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 6, 81 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 8, 81 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 82 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 7, 82 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 82 });

            migrationBuilder.DeleteData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 12, 83 });

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Crops",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.UpdateData(
                table: "ProductCrops",
                keyColumns: new[] { "CropId", "ProductId" },
                keyValues: new object[] { 2, 1 },
                column: "UsageInstruction",
                value: "Cung cấp Bo và Kẽm giúp đậu trái, chống rụng hoa.");

            migrationBuilder.InsertData(
                table: "ProductCrops",
                columns: new[] { "CropId", "ProductId", "UsageInstruction" },
                values: new object[,]
                {
                    { 3, 2, "Thuốc sinh học an toàn trừ sâu tơ bắp cải." },
                    { 1, 3, "Trừ ốc bươu vàng hại lúa non." },
                    { 2, 4, "Kích thích ra hoa sớm, đồng loạt." },
                    { 1, 8, "Giữ xanh lá đòng, hạt lúa sáng mẩy." },
                    { 3, 9, "Kích rễ rau màu phát triển mạnh." },
                    { 2, 22, "Cung cấp dinh dưỡng NPK nuôi trái lớn." },
                    { 4, 24, "Phân bón đa năng giúp phục hồi cây cà phê sau thu hoạch." },
                    { 1, 47, "Đặc trị bệnh đạo ôn trên lúa." },
                    { 1, 50, "Đặc trị rầy nâu, sâu đục thân hại lúa." },
                    { 2, 60, "Phòng trị bệnh thán thư sầu riêng, nứt thân xì mủ." },
                    { 3, 78, "Đặc trị bệnh phấn trắng trên dưa leo, bầu bí." },
                    { 4, 81, "Trừ cỏ dại trong vườn hồ tiêu, cà phê." }
                });
        }
    }
}
