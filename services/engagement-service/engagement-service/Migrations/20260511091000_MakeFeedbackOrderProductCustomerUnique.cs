using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace engagement_service.Migrations
{
    /// <inheritdoc />
    public partial class MakeFeedbackOrderProductCustomerUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_OrderId_ProductId_CustomerId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_OrderId_ProductId_CustomerId",
                table: "Feedbacks",
                columns: new[] { "OrderId", "ProductId", "CustomerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_OrderId_ProductId_CustomerId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_OrderId_ProductId_CustomerId",
                table: "Feedbacks",
                columns: new[] { "OrderId", "ProductId", "CustomerId" });
        }
    }
}
