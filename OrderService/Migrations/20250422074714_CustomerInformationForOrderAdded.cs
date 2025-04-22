using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class CustomerInformationForOrderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_Address",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_Country",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_FullName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_PhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery_CustomerInformation_ZIPCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_Address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_Email",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_FullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_PhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery_CustomerInformation_ZIPCode",
                table: "Orders");
        }
    }
}
