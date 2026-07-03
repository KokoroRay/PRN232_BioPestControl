using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace catalog_service.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductImages : Migration
    {
        // Base URL for product images (Cloudinary or any CDN).
        // Change this prefix if you host images elsewhere.
        private const string BaseUrl = "https://res.cloudinary.com/biopestcontrol/image/upload/products";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Map: (productId, sku) -> ImageUrl
            var images = new (int Id, string Sku)[]
            {
                (1,  "sp0001"),
                (2,  "sp0002"),
                (3,  "sp0003"),
                (4,  "sp0004"),
                (5,  "sp0005"),
                (6,  "sp0006"),
                (7,  "sp0007"),
                (8,  "sp0008"),
                (9,  "sp0009"),
                (10, "sp0010"),
                (11, "sp0011"),
                (12, "sp0012"),
                (13, "sp0013"),
                (14, "sp0014"),
                (15, "sp0015"),
                (16, "sp0016"),
                (17, "sp0017"),
                (18, "sp0018"),
                (19, "sp0019"),
                (20, "sp0020"),
                (21, "sp0021"),
                (22, "sp0022"),
                (23, "sp0023"),
                (24, "sp0024"),
                (25, "sp0025"),
                (26, "sp0026"),
                (27, "sp0027"),
                (28, "sp0028"),
                (29, "sp0029"),
                (30, "sp0030"),
                (31, "sp0031"),
                (32, "sp0032"),
                (33, "sp0033"),
                (34, "sp0034"),
                (35, "sp0035"),
                (36, "sp0036"),
                (37, "sp0037"),
                (38, "sp0038"),
                (39, "sp0039"),
                (40, "sp0040"),
                (41, "sp0041"),
                (42, "sp0042"),
                (43, "sp0043"),
                (44, "sp0044"),
                (45, "sp0045"),
                (46, "sp0046"),
                (47, "sp0047"),
                (48, "sp0048"),
                (49, "sp0049"),
                (50, "sp0050"),
                (51, "sp0051"),
                (52, "sp0052"),
                (53, "sp0053"),
                (54, "sp0054"),
                (55, "sp0055"),
                (56, "sp0056"),
                (57, "sp0057"),
                (58, "sp0058"),
                (59, "sp0059"),
                (60, "sp0060"),
                (61, "sp0061"),
                (62, "sp0062"),
                (63, "sp0063"),
                (64, "sp0064"),
                (65, "sp0065"),
                (66, "sp0066"),
                (67, "sp0067"),
                (68, "sp0068"),
                (69, "sp0069"),
                (70, "sp0070"),
                (71, "sp0071"),
                (72, "sp0072"),
                (73, "sp0073"),
                (74, "sp0074"),
                (75, "sp0075"),
                (76, "sp0076"),
                (77, "sp0077"),
                (78, "sp0078"),
                (79, "sp0079"),
                (80, "sp0080"),
                (81, "sp0081"),
                (82, "sp0082"),
                (83, "sp0083"),
            };

            foreach (var (id, sku) in images)
            {
                migrationBuilder.UpdateData(
                    table: "Products",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "ImageUrl",
                    value: $"{BaseUrl}/{sku}.jpg");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reset all ImageUrl back to null
            for (int id = 1; id <= 83; id++)
            {
                migrationBuilder.UpdateData(
                    table: "Products",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "ImageUrl",
                    value: null);
            }
        }
    }
}
