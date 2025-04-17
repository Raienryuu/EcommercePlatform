using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryModelIsSimilar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandlerName",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "DeliveryId",
                keyValue: new Guid("4e627d51-4510-4567-aee6-3830a25e040c"),
                column: "HandlerName",
                value: "dhl");

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "DeliveryId",
                keyValue: new Guid("8c68b176-1401-4373-aed8-3bad2f7c0f29"),
                column: "HandlerName",
                value: "dhl");

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "DeliveryId",
                keyValue: new Guid("b532c6c3-0696-4536-98a5-f1dcdf4df954"),
                column: "HandlerName",
                value: "dhl");

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "DeliveryId",
                keyValue: new Guid("dd6b0c88-538a-4ea2-877b-6143fab14ca5"),
                column: "HandlerName",
                value: "dhl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandlerName",
                table: "Deliveries");
        }
    }
}
