using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using ProductService.Tests.Fakes;
using ProductService.Utility;

namespace ProductService.Tests.Unit;

public class ProductPaginationTests : System.IDisposable
{
  private readonly SqliteConnection _connection;
  private readonly ProductDbContextFake _context;

  public ProductPaginationTests()
  {
    _connection = new SqliteConnection("Filename=:memory:");
    _connection.Open();
    var builder = new DbContextOptionsBuilder<ProductDbContextFake>().UseSqlite(_connection);
    _context = new ProductDbContextFake(builder.Options);
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

  private static void SeedData(ProductDbContextFake context)
  {
    var root = new ProductCategory { Id = 1, CategoryName = "Root" };
    context.ProductCategories.Add(root);
    context.SaveChanges();
    var child1 = new ProductCategory { Id = 2, CategoryName = "Child1", ParentCategory = root };
    var child2 = new ProductCategory { Id = 3, CategoryName = "Child2", ParentCategory = root };
    context.ProductCategories.AddRange(child1, child2);
    context.SaveChanges();
    var grandchild = new ProductCategory { Id = 4, CategoryName = "Grandchild", ParentCategory = child1 };
    context.ProductCategories.Add(grandchild);
    context.SaveChanges();

    var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), CategoryId = 1, Name = "RootProduct", Description = "", Price = 1, Quantity = 1, ConcurrencyStamp = 0 },
            new() { Id = Guid.NewGuid(), CategoryId = 2, Name = "Child1Product", Description = "", Price = 2, Quantity = 2, ConcurrencyStamp = 0 },
            new() { Id = Guid.NewGuid(), CategoryId = 3, Name = "Child2Product", Description = "", Price = 3, Quantity = 3, ConcurrencyStamp = 0 },
            new() { Id = Guid.NewGuid(), CategoryId = 4, Name = "GrandchildProduct", Description = "", Price = 4, Quantity = 4, ConcurrencyStamp = 0 },
        };
    context.Products.AddRange(products);
    context.SaveChanges();
  }

  [Fact]
  public void ProductsPagination_RootCategory_ReturnsAllDescendants()
  {
    int rootCategoryId = 1;
    var filters = new PaginationParams { Category = rootCategoryId, PageNum = 1, PageSize = 10 };
    var pagination = new ProductsPagination(filters, _context);
    var products = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();
    Assert.Equal(4, products.Count);
    Assert.Contains(products, p => p.Name == "RootProduct");
    Assert.Contains(products, p => p.Name == "Child1Product");
    Assert.Contains(products, p => p.Name == "Child2Product");
    Assert.Contains(products, p => p.Name == "GrandchildProduct");
  }

  [Fact]
  public void ProductsPagination_LeafCategory_ReturnsOnlyLeafProduct()
  {
    int leafCategoryId = 4;
    var filters = new PaginationParams { Category = leafCategoryId, PageNum = 1, PageSize = 10 };
    var pagination = new ProductsPagination(filters, _context);
    var products = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();
    Assert.Single(products);
    Assert.Equal("GrandchildProduct", products[0].Name);
  }

  [Fact]
  public void ProductsPagination_NonExistentCategory_ReturnsNoProducts()
  {
    int nonExistentCategoryId = 999;
    var filters = new PaginationParams { Category = nonExistentCategoryId, PageNum = 1, PageSize = 10 };
    var pagination = new ProductsPagination(filters, _context);
    var products = pagination.GetOffsetPageQuery(filters.PageNum, filters.PageSize).ToList();
    Assert.Empty(products);
  }

  [Fact]
  public void ProductsPagination_Pagination_WorksCorrectly()
  {
    int rootCategoryId = 1;
    var filters = new PaginationParams { Category = rootCategoryId, PageNum = 1, PageSize = 2 };
    var pagination = new ProductsPagination(filters, _context);
    var page1 = pagination.GetOffsetPageQuery(1, 2).ToList();
    var page2 = pagination.GetOffsetPageQuery(2, 2).ToList();
    Assert.Equal(2, page1.Count);
    Assert.Equal(2, page2.Count);
    var allNames = page1.Concat(page2).Select(p => p.Name).ToList();
    Assert.Contains("RootProduct", allNames);
    Assert.Contains("Child1Product", allNames);
    Assert.Contains("Child2Product", allNames);
    Assert.Contains("GrandchildProduct", allNames);
  }
}