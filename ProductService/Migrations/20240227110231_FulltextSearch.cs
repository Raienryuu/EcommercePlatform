using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Migrations
{
  /// <inheritdoc />
  public partial class FulltextSearch : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.UpdateData(
        table: "Products",
        keyColumn: "Id",
        keyValue: 1,
        column: "ConcurrencyStamp",
        value: null);

      migrationBuilder.UpdateData(
        table: "Products",
        keyColumn: "Id",
        keyValue: 2,
        column: "ConcurrencyStamp",
        value: null);

      migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs as f WHERE f.name = 'fulltextProduct')" +
                           "\nBEGIN" +
                           "\n\tCREATE FULLTEXT CATALOG fulltextProduct AS DEFAULT;" +
                           "\nEND", true);
      migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Products(\"Name\")" +
                           "KEY INDEX PK_Products;", true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
  }
}