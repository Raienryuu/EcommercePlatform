using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class ReservingOrdersProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "Mugs");

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "CategoryName", "ParentCategoryId" },
                values: new object[,]
                {
                    { 3, "Cups", null },
                    { 4, "Electronics", null },
                    { 5, "Laptops", null }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "ConcurrencyStamp" },
                values: new object[] { 2, new byte[] { 58, 127, 143, 48 } });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "ConcurrencyStamp", "Description", "Name", "Price" },
                values: new object[] { 3, new byte[] { 197, 70, 88, 107 }, "White cup", "Cpu", 10m });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "ConcurrencyStamp", "Description", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 3, 4, new byte[] { 246, 32, 9, 103 }, "Apple laptop", "MacBook Air M2", 1000m, 100 },
                    { 4, 4, new byte[] { 57, 5, 95, 196 }, "Apple laptop", "Lenovo ThindPad E14", 1000m, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "Electronics");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "ConcurrencyStamp" },
                values: new object[] { 1, new byte[] { 5, 42, 36, 105 } });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "ConcurrencyStamp", "Description", "Name", "Price" },
                values: new object[] { 2, new byte[] { 233, 121, 171, 107 }, "Apple laptop", "Laptop", 1000m });
        }
    }
}
