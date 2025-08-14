using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Services;
using Xunit;

namespace ProductService.Tests.Unit;

public class ProductServiceTests : IDisposable
{
  private readonly DbContextOptions<ProductDbContext> _options;
  private readonly ProductDbContext _context;
  private readonly Services.ProductService _productService;

  public ProductServiceTests()
  {
    var connection = new SqliteConnection("datasource=:memory:");
    connection.Open();
    _options = new DbContextOptionsBuilder<ProductDbContext>().UseSqlite(connection).Options;

    _context = new ProductDbContext(_options);
    _context.Database.EnsureCreated();

    _context.ProductCategories.AddRange(
      new ProductCategory { CategoryName = "Category1" },
      new ProductCategory { CategoryName = "Category2" }
    );
    _context.SaveChanges();

    _productService = new Services.ProductService(_context);
  }

  public void Dispose()
  {
    _context.Database.EnsureDeleted();
    _context.Dispose();
    GC.SuppressFinalize(this);
  }

  private async Task<ProductCategory> CreateCategory()
  {
    var category = new ProductCategory { CategoryName = "Test Category" };
    _context.ProductCategories.Add(category);
    await _context.SaveChangesAsync();
    return category;
  }

  private async Task<Product> CreateProduct(int categoryId)
  {
    var product = new Product
    {
      Id = Guid.NewGuid(),
      Name = "TestProduct",
      Description = "Test Description",
      Price = 10.00m,
      CategoryId = categoryId,
      Quantity = 10,
      ConcurrencyStamp = 1,
    };
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
    return product;
  }

  [Fact]
  public async Task AddProduct_ValidProduct_ReturnsSuccess()
  {
    var category = await CreateCategory();
    var newProduct = new Product
    {
      Id = Guid.NewGuid(),
      Name = "New Product",
      Description = "Description",
      Price = 20.00m,
      CategoryId = category.Id,
      Quantity = 5,
      ConcurrencyStamp = 1,
    };

    var result = await _productService.AddProduct(newProduct);

    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.Created, (int)result.StatusCode);
    Assert.NotNull(result.Value);

    var productInDb = await _context.Products.FindAsync(newProduct.Id);
    Assert.NotNull(productInDb);
    Assert.Equal("New Product", productInDb.Name);
    Assert.Equal(5, productInDb.Quantity);
  }

  [Fact]
  public async Task AddProduct_CategoryNotFound_ReturnsNotFound()
  {
    var newProduct = new Product
    {
      Id = Guid.NewGuid(),
      Name = "New Product",
      Description = "Description",
      Price = 20.00m,
      CategoryId = -1, // Non-existent category ID
      Quantity = 10,
      ConcurrencyStamp = 1,
    };

    var result = await _productService.AddProduct(newProduct);

    Assert.False(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.NotFound, (int)result.StatusCode);
    Assert.Equal("Category not found", result.ErrorMessage);
  }

  [Fact]
  public async Task GetBatchProducts_ExistingProductIds_ReturnsProducts()
  {
    var category = await CreateCategory();
    var product1 = await CreateProduct(category.Id);
    var product2 = await CreateProduct(category.Id);

    var productIds = new List<Guid> { product1.Id, product2.Id };
    var result = await _productService.GetBatchProducts(productIds);

    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.OK, (int)result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal(2, result.Value.Count);
  }

  [Fact]
  public async Task GetBatchProducts_SomeProductsNotFound_ReturnsNotFound()
  {
    var category = await CreateCategory();
    var product1 = await CreateProduct(category.Id);
    var nonExistentProductId = Guid.NewGuid();

    var productIds = new List<Guid> { product1.Id, nonExistentProductId };
    var result = await _productService.GetBatchProducts(productIds);

    Assert.False(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.NotFound, (int)result.StatusCode);
    Assert.Equal("Some products id were not found", result.ErrorMessage);
  }

  [Fact]
  public async Task GetBatchProducts_EmptyList_ReturnsEmptyList()
  {
    var productIds = new List<Guid>();
    var result = await _productService.GetBatchProducts(productIds);

    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.OK, (int)result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Empty(result.Value);
  }

  [Fact]
  public async Task GetProduct_ExistingProductId_ReturnsProduct()
  {
    var category = await CreateCategory();
    var product = await CreateProduct(category.Id);

    var result = await _productService.GetProduct(product.Id);

    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.OK, (int)result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal(product.Name, result.Value.Name);
  }

  [Fact]
  public async Task GetProduct_NonExistentProductId_ReturnsNotFound()
  {
    var nonExistentProductId = Guid.NewGuid();
    var result = await _productService.GetProduct(nonExistentProductId);

    Assert.False(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.NotFound, (int)result.StatusCode);
    Assert.Equal($"No product found with given ID: {nonExistentProductId}.", result.ErrorMessage);
  }

  [Fact]
  public async Task UpdateProduct_ValidProduct_ReturnsSuccess()
  {
    var category = await CreateCategory();
    var product = await CreateProduct(category.Id);
    _context.ChangeTracker.Clear();

    var updatedProduct = new Product
    {
      Id = product.Id,
      Name = "Updated Product Name",
      Description = "Updated Description",
      Price = 25.00m,
      CategoryId = category.Id,
      Quantity = 15,
      ConcurrencyStamp = product.ConcurrencyStamp,
    };

    var result = await _productService.UpdateProduct(updatedProduct);

    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.OK, (int)result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal("Updated Product Name", result.Value.Name);

    var productInDb = await _context.Products.FindAsync(product.Id);
    Assert.NotNull(productInDb);
    Assert.Equal("Updated Product Name", productInDb.Name);
    Assert.Equal(15, productInDb.Quantity);
  }

  [Fact]
  public async Task UpdateProduct_NonExistentProduct_ReturnsNotFound()
  {
    var nonExistentProductId = Guid.NewGuid();
    var updatedProduct = new Product
    {
      Id = nonExistentProductId,
      Name = "Updated Product Name",
      Description = "Updated Description",
      Price = 25.00m,
      CategoryId = -1,
      Quantity = 10,
      ConcurrencyStamp = 1,
    };

    var result = await _productService.UpdateProduct(updatedProduct);

    Assert.False(result.IsSuccess);
    Assert.Equal(404, (int)result.StatusCode);
    Assert.Equal("Product not found", result.ErrorMessage);
  }

  [Fact]
  public async Task UpdateProduct_CategoryNotFound_ReturnsNotFound()
  {
    var category = await CreateCategory();
    var product = await CreateProduct(category.Id);
    _context.ChangeTracker.Clear();

    var updatedProduct = new Product
    {
      Id = product.Id,
      Name = "Updated Product Name",
      Description = "Updated Description",
      Price = 25.00m,
      CategoryId = -1, // Non-existent category
      Quantity = 10,
      ConcurrencyStamp = product.ConcurrencyStamp,
    };

    var result = await _productService.UpdateProduct(updatedProduct);

    Assert.False(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.NotFound, (int)result.StatusCode);
    Assert.Equal("Given category does not exists", result.ErrorMessage);
  }

  [Fact]
  public async Task UpdateProduct_ConcurrencyStampMismatch_ReturnsConcurrencyError()
  {
    var category = await CreateCategory();
    var product = await CreateProduct(category.Id);
    _context.ChangeTracker.Clear();

    var updatedProduct = new Product
    {
      Id = product.Id,
      Name = "Updated Product Name",
      Description = "Updated Description",
      Price = 25.00m,
      CategoryId = category.Id,
      Quantity = 10,
      ConcurrencyStamp = product.ConcurrencyStamp + 1, // Different ConcurrencyStamp
    };

    var result = await _productService.UpdateProduct(updatedProduct);

    Assert.False(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.UnprocessableEntity, (int)result.StatusCode);
    Assert.Equal("ConcurrencyStamp mismatch", result.ErrorMessage);
  }

  [Fact]
  public async Task UpdateProduct_DifferentValuesProvided_ReturnsUpdatedValues()
  {
    // Arrange
    var category = await CreateCategory();
    var product = await CreateProduct(category.Id);
    _context.ChangeTracker.Clear();

    var updatedProduct = new Product
    {
      Id = product.Id,
      Name = "Different Product Name",
      Description = "Different Description",
      Price = 35.00m,
      CategoryId = category.Id,
      Quantity = 20,
      ConcurrencyStamp = product.ConcurrencyStamp,
    };

    // Act
    var result = await _productService.UpdateProduct(updatedProduct);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal((int)System.Net.HttpStatusCode.OK, (int)result.StatusCode);

    var productInDb = await _context.Products.FindAsync(product.Id);
    Assert.NotNull(productInDb);
    Assert.Equal("Different Product Name", productInDb.Name);
    Assert.Equal("Different Description", productInDb.Description);
    Assert.Equal(35.00m, productInDb.Price);
    Assert.Equal(20, productInDb.Quantity);
  }
}
