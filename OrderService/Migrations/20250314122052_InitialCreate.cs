using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "Orders",
        columns: table => new
        {
          OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
          Status = table.Column<int>(type: "int", nullable: false),
          Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
          Created = table.Column<DateTime>(type: "datetime2", nullable: false),
          LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
          StripePaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Orders", x => x.OrderId);
        }
      );

      migrationBuilder.CreateTable(
        name: "StagedCarts",
        columns: table => new
        {
          OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_StagedCarts", x => x.OwnerId);
        }
      );

      migrationBuilder.CreateTable(
        name: "Orders_Products",
        columns: table => new
        {
          OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
          ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          Quantity = table.Column<int>(type: "int", nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Orders_Products", x => new { x.OrderId, x.Id });
          table.ForeignKey(
            name: "FK_Orders_Products_Orders_OrderId",
            column: x => x.OrderId,
            principalTable: "Orders",
            principalColumn: "OrderId",
            onDelete: ReferentialAction.Cascade
          );
        }
      );

      migrationBuilder.CreateTable(
        name: "StagedCarts_Products",
        columns: table => new
        {
          StagedCartOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
          ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          Quantity = table.Column<int>(type: "int", nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_StagedCarts_Products", x => new { x.StagedCartOwnerId, x.Id });
          table.ForeignKey(
            name: "FK_StagedCarts_Products_StagedCarts_StagedCartOwnerId",
            column: x => x.StagedCartOwnerId,
            principalTable: "StagedCarts",
            principalColumn: "OwnerId",
            onDelete: ReferentialAction.Cascade
          );
        }
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: "Orders_Products");

      migrationBuilder.DropTable(name: "StagedCarts_Products");

      migrationBuilder.DropTable(name: "Orders");

      migrationBuilder.DropTable(name: "StagedCarts");
    }
  }
}
