using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility;

namespace ProductService.Tests.Utility
{
  public class ProductPaginationTests : IDisposable
  {
    private readonly SqliteConnection _connection;
    private readonly ProductDbContext _context;

    public ProductPaginationTests()
    {
      _connection = new SqliteConnection("Filename=:memory:");
      _connection.Open();

      var builder = new DbContextOptionsBuilder<ProductDbContext>().UseSqlite(_connection);

      _context = new ProductDbContext(builder.Options);
      _context.Database.EnsureCreated();
      _context.ProductCategories.Where(_ => true).ExecuteDelete();
      _context.Products.Where(_ => true).ExecuteDelete();

      SeedData(_context);
    }

    public void Dispose()
    {
      _connection.Close();
      GC.SuppressFinalize(this);
    }

    private static void SeedData(ProductDbContext context)
    {
      var categories = new List<ProductCategory>
      {
        new() { Id = 1, CategoryName = "Category 1" },
        new() { Id = 2, CategoryName = "Category 2" },
      };
      context.ProductCategories.AddRange(categories);
      context.SaveChanges();

      var products = new List<Product>
      {
        new()
        {
          Id = Guid.NewGuid(),
          CategoryId = 1,
          Name = "Product A",
          Description = "Description A",
          Price = 10.00m,
          Quantity = 100,
          ConcurrencyStamp = 0,
        },
        new()
        {
          Id = Guid.NewGuid(),
          CategoryId = 1,
          Name = "Product B",
          Description = "Description B",
          Price = 20.00m,
          Quantity = 200,
          ConcurrencyStamp = 0,
        },
        new()
        {
          Id = Guid.NewGuid(),
          CategoryId = 2,
          Name = "Product C",
          Description = "Description C",
          Price = 15.00m,
          Quantity = 150,
          ConcurrencyStamp = 0,
        },
        new()
        {
          Id = Guid.NewGuid(),
          CategoryId = 2,
          Name = "Product D",
          Description = "Description D",
          Price = 25.00m,
          Quantity = 50,
          ConcurrencyStamp = 0,
        },
        new()
        {
          Id = Guid.NewGuid(),
          CategoryId = 1,
          Name = "Product E",
          Description = "Description E",
          Price = 12.00m,
          Quantity = 120,
          ConcurrencyStamp = 0,
        },
      };
      context.Products.AddRange(products);
      context.SaveChanges();
    }

    [Fact]
    public void GetOffsetPageQuery_NoFilters_ReturnsFirstPage()
    {
      var filters = new PaginationParams { PageNum = 1, PageSize = 2 };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_PriceAscending_ReturnsOrderedPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Order = PaginationParams.SortType.PriceAsc,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_PriceDescending_ReturnsOrderedPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Order = PaginationParams.SortType.PriceDesc,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product D", result[0].Name);
      Assert.Equal("Product B", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_QuantityAscending_ReturnsOrderedPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Order = PaginationParams.SortType.QuantityAsc,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product D", result[0].Name);
      Assert.Equal("Product A", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_NameFilter_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Name = "Product A",
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Single(result);
      Assert.Equal("Product A", result[0].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_MinPriceFilter_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        MinPrice = 15.00m,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product B", result[0].Name);
      Assert.Equal("Product C", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_MaxPriceFilter_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        MaxPrice = 15.00m,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_MinQuantityFilter_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        MinQuantity = 150,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product B", result[0].Name);
      Assert.Equal("Product C", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_CategoryFilter_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Category = 1,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetOffsetPageQuery_CombinedFilters_ReturnsFilteredPage()
    {
      var filters = new PaginationParams
      {
        PageNum = 1,
        PageSize = 2,
        Category = 1,
        MaxPrice = 15.00m,
      };
      var pagination = new ProductsPagination(filters, _context);
      var result = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetNextPageQuery_NoFilters_ReturnsNextPage()
    {
      var filters = new PaginationParams { PageSize = 2, Order = PaginationParams.SortType.PriceAsc };
      var pagination = new ProductsPagination(filters, _context);

      // Get the first product from the first page
      var firstPage = pagination.GetOffsetPageQuery(1, filters.PageSize).ToList();
      Assert.Equal(2, firstPage.Count); // Ensure first page exists and has 2 elements
      var firstProduct = firstPage[1]; // get last element of first page

      var result = pagination.GetNextPageQuery(filters.PageSize, firstProduct).ToList();
      Assert.Equal(2, result.Count);
      Assert.Equal("Product B", result[0].Name);
      Assert.Equal("Product C", result[1].Name);
    }

    [Fact]
    public void GetNextPageQuery_PriceDescending_ReturnsNextPageInCorrectOrder()
    {
      var filters = new PaginationParams { PageSize = 2, Order = PaginationParams.SortType.PriceDesc };
      var pagination = new ProductsPagination(filters, _context);

      var firstPage = pagination.GetOffsetPageQuery(1, filters.PageSize).ToList();
      Assert.Equal(2, firstPage.Count);
      var firstProduct = firstPage[1];

      var result = pagination.GetNextPageQuery(filters.PageSize, firstProduct).ToList();
      Assert.Equal(2, result.Count);
      Assert.Equal("Product A", result[0].Name);
      Assert.Equal("Product E", result[1].Name);
    }

    [Fact]
    public void GetPreviousPageQuery_NoFilters_ReturnsPreviousPage()
    {
      var filters = new PaginationParams { PageSize = 2, Order = PaginationParams.SortType.PriceAsc };
      var pagination = new ProductsPagination(filters, _context);

      // Get the last product from a later page (e.g., page 3 if it existed). We'll use page 2 since we only have 5 products
      var secondPage = pagination.GetOffsetPageQuery(2, filters.PageSize).ToList();
      Assert.Single(secondPage); //Ensure the second page exists with 1 element

      var lastProduct = secondPage[0];
      var result = pagination.GetPreviousPageQuery(filters.PageSize, lastProduct).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product E", result[0].Name);
      Assert.Equal("Product A", result[1].Name);
    }

    [Fact]
    public void GetPreviousPageQuery_PriceDescending_ReturnsPreviousPageInCorrectOrder()
    {
      var filters = new PaginationParams { PageSize = 2, Order = PaginationParams.SortType.PriceDesc };
      var pagination = new ProductsPagination(filters, _context);

      var secondPage = pagination.GetOffsetPageQuery(2, filters.PageSize).ToList();
      Assert.Single(secondPage);

      var lastProduct = secondPage[0];
      var result = pagination.GetPreviousPageQuery(filters.PageSize, lastProduct).ToList();

      Assert.Equal(2, result.Count);
      Assert.Equal("Product B", result[0].Name);
      Assert.Equal("Product D", result[1].Name);
    }
  }
}
