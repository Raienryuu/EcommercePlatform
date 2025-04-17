using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
  /// <inheritdoc />
  public partial class DeliveryIdIsNotEqualOrderId : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<Guid>(
        name: "Delivery_DeliveryId",
        table: "Orders",
        type: "uniqueidentifier",
        nullable: true
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(name: "Delivery_DeliveryId", table: "Orders");
    }
  }
}
