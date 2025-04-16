using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class PaymentAndDeliveryTypeOnDelivery : Migration
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
                    DeliveryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryId);
                });

            migrationBuilder.CreateTable(
                name: "OrdersReserved",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsReserved = table.Column<bool>(type: "bit", nullable: false),
                    ReserveTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersReserved", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "DeliveryId", "DeliveryType", "Name", "PaymentType", "Price" },
                values: new object[,]
                {
                    { new Guid("4e627d51-4510-4567-aee6-3830a25e040c"), "DeliveryPoint", "DHL Parcel Locker", "Online", 0m },
                    { new Guid("8c68b176-1401-4373-aed8-3bad2f7c0f29"), "DirectCustomerAddress", "Premium shipping", "Online", 18m },
                    { new Guid("b532c6c3-0696-4536-98a5-f1dcdf4df954"), "DirectCustomerAddress", "Standard shipping (cash)", "Cash", 20m },
                    { new Guid("dd6b0c88-538a-4ea2-877b-6143fab14ca5"), "DirectCustomerAddress", "Standard shipping", "Online", 9m }
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "CategoryName", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, "Mugs", null },
                    { 2, "Tableware", null },
                    { 3, "Cups", null },
                    { 4, "Electronics", null },
                    { 5, "Laptops", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "ConcurrencyStamp", "Description", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { new Guid("22ea176b-ea99-445f-97b3-c1afa5585562"), 3, 0, "White cup", "Cpu", 10m, 100 },
                    { new Guid("87817c15-d25f-4621-9135-2e7851b484b3"), 2, 0, "White mug", "Mug", 10m, 100 },
                    { new Guid("d8f65542-f0e7-4ca7-b9c1-002898cdc379"), 4, 0, "Apple laptop", "Lenovo ThindPad E14", 1000m, 100 },
                    { new Guid("f12e47a0-82f9-4231-abce-63280e7d3d99"), 4, 0, "Apple laptop", "MacBook Air M2", 1000m, 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryName",
                table: "ProductCategories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Quantity",
                table: "Products",
                column: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "OrdersReserved");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
