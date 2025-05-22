using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
  /// <inheritdoc />
  public partial class ExplicitPriceTypeSet : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<decimal>(
        name: "Delivery_Price",
        table: "Orders",
        type: "money",
        nullable: true,
        oldClrType: typeof(decimal),
        oldType: "decimal(18,2)",
        oldNullable: true
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<decimal>(
        name: "Delivery_Price",
        table: "Orders",
        type: "decimal(18,2)",
        nullable: true,
        oldClrType: typeof(decimal),
        oldType: "money",
        oldNullable: true
      );
    }
  }
}
