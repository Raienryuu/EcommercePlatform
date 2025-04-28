using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class SeededRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("5ad84e59-945b-463a-bfa5-a10834aacb14"), "163", "Moderator", "MODERATOR" },
                    { new Guid("71d4e31e-3e7d-4194-b213-be5d4a4630af"), "163", "Administrator", "ADMINISTRATOR" },
                    { new Guid("89a59295-562a-4c0b-9d52-1d3c07da4afe"), "123", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5ad84e59-945b-463a-bfa5-a10834aacb14"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("71d4e31e-3e7d-4194-b213-be5d4a4630af"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("89a59295-562a-4c0b-9d52-1d3c07da4afe"));
        }
    }
}
