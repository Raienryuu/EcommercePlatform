using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeliveryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryId);
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "DeliveryId", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("4e627d51-4510-4567-aee6-3830a25e040c"), "DHL Parcel Locker", 0m },
                    { new Guid("8c68b176-1401-4373-aed8-3bad2f7c0f29"), "Premium shipping", 18m },
                    { new Guid("b532c6c3-0696-4536-98a5-f1dcdf4df954"), "Standard shipping (cash)", 20m },
                    { new Guid("dd6b0c88-538a-4ea2-877b-6143fab14ca5"), "Standard shipping", 9m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");
        }
    }
}
