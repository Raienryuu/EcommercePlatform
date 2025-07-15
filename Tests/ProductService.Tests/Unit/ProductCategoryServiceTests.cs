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

    // Seed data for GET methods tests
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
    var categoryId = 4; // Define a new id that isn't in seed data
    var newCategory = new ProductCategory() { Id = categoryId, CategoryName = "Original Category" };
    await _productCategoryService.CreateProductCategory(newCategory, _cancellationToken);

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

    var updatedEntity = await _productCategoryService.GetProductCategory(categoryId, _cancellationToken);
    Assert.Equal("Updated Category", updatedEntity.Value.CategoryName);
  }

  [Fact]
  public async Task UpdateProductCategory_WhenIdsDoNotMatch_ReturnsBadRequest()
  {
    // Arrange
    var categoryId = 5; // Define a new id that isn't in seed data
    var otherId = 6;

    // Create a separate context and add a test category

    var newCategory = new ProductCategory() { Id = categoryId, CategoryName = "Original Category" };

    await _productCategoryService.CreateProductCategory(newCategory, _cancellationToken);

    var updatedCategory = new ProductCategory() { Id = otherId, CategoryName = "Updated Category" };

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
    var newCategory = new ProductCategory() { CategoryName = "NewCategoryTest" };

    // Act
    var result = await _productCategoryService.CreateProductCategory(newCategory, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(201, result.StatusCode);
    Assert.Equal("NewCategoryTest", result.Value.CategoryName);

    var createdEntity = await _context.ProductCategories.FindAsync(result.Value.Id);
    Assert.NotNull(createdEntity);
    Assert.Equal("NewCategoryTest", createdEntity.CategoryName);
  }

  [Fact]
  public async Task CreateProductCategory_WhenCategoryAlreadyExists_ReturnsConflict()
  {
    // Arrange
    // The seed data already created "Category 1", so that we could retrieve data. Now we use this for conflict
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
    var categoryId = 7; // Define a new category to Delete
    var newCategory = new ProductCategory() { Id = categoryId, CategoryName = "ToBeDeleted" };
    await _productCategoryService.CreateProductCategory(newCategory, _cancellationToken);

    // Act
    var result = await _productCategoryService.DeleteProductCategory(categoryId, _cancellationToken);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(204, result.StatusCode);

    var deletedEntity = await _productCategoryService.GetProductCategory(categoryId, _cancellationToken);
    Assert.False(deletedEntity.IsSuccess);
    Assert.Equal(404, deletedEntity.StatusCode);
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
