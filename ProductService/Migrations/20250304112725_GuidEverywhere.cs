using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class GuidEverywhere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<int>(
                name: "ConcurrencyStamp",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryName",
                value: "Mugs");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "Tableware");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("22ea176b-ea99-445f-97b3-c1afa5585562"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("87817c15-d25f-4621-9135-2e7851b484b3"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d8f65542-f0e7-4ca7-b9c1-002898cdc379"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f12e47a0-82f9-4231-abce-63280e7d3d99"));

            migrationBuilder.AlterColumn<byte[]>(
                name: "ConcurrencyStamp",
                table: "Products",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryName",
                value: "Tableware");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryName",
                value: "Mugs");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "ConcurrencyStamp", "Description", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 2, new byte[] { 75, 72, 22, 190 }, "White mug", "Mug", 10m, 100 },
                    { 2, 3, new byte[] { 236, 99, 131, 91 }, "White cup", "Cpu", 10m, 100 },
                    { 3, 4, new byte[] { 139, 99, 223, 24 }, "Apple laptop", "MacBook Air M2", 1000m, 100 },
                    { 4, 4, new byte[] { 4, 108, 99, 39 }, "Apple laptop", "Lenovo ThindPad E14", 1000m, 100 }
                });
        }
    }
}
