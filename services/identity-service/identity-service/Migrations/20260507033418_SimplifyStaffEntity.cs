using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace identity_service.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyStaffEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Staffs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Staffs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Staffs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Staffs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
