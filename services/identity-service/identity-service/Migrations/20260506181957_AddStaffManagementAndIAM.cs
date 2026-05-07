using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace identity_service.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffManagementAndIAM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GroupCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsFullAccess = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staffs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffPermissions_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Description", "DisplayName", "DisplayOrder", "GroupCode", "GroupName", "IsActive" },
                values: new object[,]
                {
                    { 1, "customers.view_list", null, "Xem danh sách khách hàng", 1, "UC14", "UC14 - Quản lý khách hàng", true },
                    { 2, "customers.view_detail", null, "Xem chi tiết khách hàng", 2, "UC14", "UC14 - Quản lý khách hàng", true },
                    { 3, "customers.search", null, "Tìm kiếm khách hàng", 3, "UC14", "UC14 - Quản lý khách hàng", true },
                    { 4, "customers.edit", null, "Chỉnh sửa khách hàng", 4, "UC14", "UC14 - Quản lý khách hàng", true },
                    { 5, "customers.lock", null, "Khóa tài khoản khách hàng", 5, "UC14", "UC14 - Quản lý khách hàng", true },
                    { 6, "orders.view_list", null, "Xem danh sách đơn hàng", 1, "UC15", "UC15 - Quản lý đơn hàng", true },
                    { 7, "orders.view_detail", null, "Xem chi tiết đơn hàng", 2, "UC15", "UC15 - Quản lý đơn hàng", true },
                    { 8, "orders.search", null, "Tìm kiếm đơn hàng", 3, "UC15", "UC15 - Quản lý đơn hàng", true },
                    { 9, "orders.update_delivery", null, "Cập nhật giao hàng", 4, "UC15", "UC15 - Quản lý đơn hàng", true },
                    { 10, "articles.filter", null, "Lọc bài viết / tin tức", 1, "UC16", "UC16 - Quản lý bài viết", true },
                    { 11, "articles.view", null, "Xem bài viết / tin tức", 2, "UC16", "UC16 - Quản lý bài viết", true },
                    { 12, "articles.create", null, "Tạo bài viết / tin tức", 3, "UC16", "UC16 - Quản lý bài viết", true },
                    { 13, "articles.edit", null, "Chỉnh sửa bài viết", 4, "UC16", "UC16 - Quản lý bài viết", true },
                    { 14, "articles.delete", null, "Xóa bài viết / tin tức", 5, "UC16", "UC16 - Quản lý bài viết", true },
                    { 15, "products.filter", null, "Lọc sản phẩm", 1, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 16, "products.search", null, "Tìm kiếm sản phẩm", 2, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 17, "products.view_detail", null, "Xem chi tiết sản phẩm", 3, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 18, "products.view_list", null, "Xem danh sách sản phẩm", 4, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 19, "products.view_feedback", null, "Xem phản hồi sản phẩm", 5, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 20, "products.reply_feedback", null, "Trả lời phản hồi sản phẩm", 6, "UC17", "UC17 - Quản lý sản phẩm", true },
                    { 21, "categories.view", null, "Xem danh mục sản phẩm", 1, "UC18", "UC18 - Quản lý danh mục", true },
                    { 22, "categories.search", null, "Tìm kiếm danh mục", 2, "UC18", "UC18 - Quản lý danh mục", true },
                    { 23, "warehouse.view", null, "Xem kho hàng", 1, "UC19", "UC19 - Quản lý kho", true },
                    { 24, "warehouse.filter", null, "Lọc kho hàng", 2, "UC19", "UC19 - Quản lý kho", true },
                    { 25, "chemical_safety.view", null, "Xem an toàn hóa chất", 1, "UC20", "UC20 - An toàn hóa chất", true },
                    { 26, "chemical_safety.search", null, "Tìm kiếm an toàn hóa chất", 2, "UC20", "UC20 - An toàn hóa chất", true },
                    { 27, "chemical_safety.filter", null, "Lọc an toàn hóa chất", 3, "UC20", "UC20 - An toàn hóa chất", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffPermissions_PermissionId",
                table: "StaffPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPermissions_StaffId_PermissionId",
                table: "StaffPermissions",
                columns: new[] { "StaffId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserId",
                table: "Staffs",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Staffs");
        }
    }
}
