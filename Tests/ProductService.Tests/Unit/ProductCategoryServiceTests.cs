using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Tests.Unit;

public class ProductCategoryServiceTests : IDisposable
{
  private readonly ProductDbContext _context;
  private readonly ProductCategoryService _productCategoryService;
  private readonly SqliteConnection _connection;
  private readonly CancellationToken _cancellationToken = CancellationToken.None;

  public ProductCategoryServiceTests()
  {
    _connection = new SqliteConnection("Filename=:memory:");
    _connection.Open();

    var options = new DbContextOptionsBuilder<ProductDbContext>().UseSqlite(_connection).Options;

    _context = new ProductDbContext(options);
    _context.Database.EnsureCreated();
    _context.ProductCategories.Where(_ => true).ExecuteDelete();

    // Seed data
    var parentCategory = new ProductCategory() { Id = 1, CategoryName = "Category 1" };
    var productCategories = new List<ProductCategory>()
    {
      parentCategory,
      new()
      {
        Id = 2,
        CategoryName = "Category 2",
        ParentCategory = parentCategory,
      },
      new() { Id = 3, CategoryName = "Category 3" },
    };

    _context.ProductCategories.AddRange(productCategories);
    _context.SaveChanges();

    _productCategoryService = new ProductCategoryService(_context);
  }

  public void Dispose()
  {
    _connection.Close();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task GetProductCategories_WhenCategoriesExist_ReturnsAllCategories()
  {
    // Act
    var result = await _productCategoryService.GetProductCategories(_cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(3, result.Value.Count);
  }

  [Fact]
  public async Task GetProductCategory_WhenCategoryExists_ReturnsCategory()
  {
    // Arrange
    var categoryId = 1;

    // Act
    var result = await _productCategoryService.GetProductCategory(categoryId, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(categoryId, result.Value.Id);
  }

  [Fact]
  public async Task GetProductCategory_WhenCategoryDoesNotExist_ReturnsNotFound()
  {
    // Arrange
    var categoryId = 99;

    // Act
    var result = await _productCategoryService.GetProductCategory(categoryId, _cancellationToken);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task GetChildrenCategories_WhenCategoryHasChildren_ReturnsChildren()
  {
    // Arrange
    var parentCategoryId = 1;

    // Act
    var result = await _productCategoryService.GetChildrenCategories(parentCategoryId, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotEmpty(result.Value);
    Assert.Single(result.Value);
    Assert.Equal(2, result.Value[0].Id);
  }

  [Fact]
  public async Task GetChildrenCategories_WhenCategoryHasNoChildren_ReturnsEmptyList()
  {
    // Arrange
    var parentCategoryId = 3;

    // Act
    var result = await _productCategoryService.GetChildrenCategories(parentCategoryId, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Empty(result.Value);
  }

  [Fact]
  public async Task GetChildrenCategories_WhenCategoryDoesNotExist_ReturnsNotFound()
  {
    // Arrange
    var parentCategoryId = 99;

    // Act
    var result = await _productCategoryService.GetChildrenCategories(parentCategoryId, _cancellationToken);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task UpdateProductCategory_WhenCategoryIsUpdated_ReturnsSuccess()
  {
    // Arrange
    var categoryId = 1;
    var updatedCategory = new ProductCategory() { Id = categoryId, CategoryName = "Updated Category" };

    // Act
    var result = await _productCategoryService.UpdateProductCategory(
      categoryId,
      updatedCategory,
      _cancellationToken
    );

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(204, result.StatusCode);

    var updatedEntity = await _context.ProductCategories.FindAsync(categoryId);
    Assert.Equal("Updated Category", updatedEntity!.CategoryName);
  }

  [Fact]
  public async Task UpdateProductCategory_WhenIdsDoNotMatch_ReturnsBadRequest()
  {
    // Arrange
    var categoryId = 1;
    var updatedCategory = new ProductCategory() { Id = 2, CategoryName = "Updated Category" };

    // Act
    var result = await _productCategoryService.UpdateProductCategory(
      categoryId,
      updatedCategory,
      _cancellationToken
    );

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(400, result.StatusCode);
  }

  [Fact]
  public async Task UpdateProductCategory_WhenCategoryDoesNotExist_ReturnsNotFound()
  {
    // Arrange
    var categoryId = 99;
    var updatedCategory = new ProductCategory() { Id = categoryId, CategoryName = "Updated Category" };

    // Act
    var result = await _productCategoryService.UpdateProductCategory(
      categoryId,
      updatedCategory,
      _cancellationToken
    );

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task CreateProductCategory_WhenCategoryIsCreated_ReturnsSuccess()
  {
    // Arrange
    var newCategory = new ProductCategory() { CategoryName = "New Category" };

    // Act
    var result = await _productCategoryService.CreateProductCategory(newCategory, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(201, result.StatusCode);
    Assert.Equal("New Category", result.Value.CategoryName);

    var createdEntity = await _context.ProductCategories.FindAsync(result.Value.Id);
    Assert.NotNull(createdEntity);
    Assert.Equal("New Category", createdEntity.CategoryName);
  }

  [Fact]
  public async Task CreateProductCategory_WhenCategoryAlreadyExists_ReturnsConflict()
  {
    // Arrange
    var existingCategory = new ProductCategory() { CategoryName = "Category 1" };

    // Act
    var result = await _productCategoryService.CreateProductCategory(existingCategory, _cancellationToken);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(409, result.StatusCode);
  }

  [Fact]
  public async Task DeleteProductCategory_WhenCategoryIsDeleted_ReturnsSuccess()
  {
    // Arrange
    var categoryId = 1;

    // Act
    var result = await _productCategoryService.DeleteProductCategory(categoryId, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(204, result.StatusCode);

    var deletedEntity = await _context.ProductCategories.FindAsync(categoryId);
    Assert.Null(deletedEntity);
  }

  [Fact]
  public async Task DeleteProductCategory_WhenCategoryDoesNotExist_ReturnsNotFound()
  {
    // Arrange
    var categoryId = 99;

    // Act
    var result = await _productCategoryService.DeleteProductCategory(categoryId, _cancellationToken);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }
}
