using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
  /// <inheritdoc />
  public partial class PaymentConfirmedToPayementStatus : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(name: "PaymentSucceded", table: "Orders");

      migrationBuilder.AddColumn<int>(
        name: "PaymentStatus",
        table: "Orders",
        type: "int",
        nullable: false,
        defaultValue: 0
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(name: "PaymentStatus", table: "Orders");

      migrationBuilder.AddColumn<bool>(
        name: "PaymentSucceded",
        table: "Orders",
        type: "bit",
        nullable: false,
        defaultValue: false
      );
    }
  }
}
