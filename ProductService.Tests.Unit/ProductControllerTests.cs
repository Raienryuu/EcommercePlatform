using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ProductService.Controllers.v1;
using ProductService.Models;

namespace ProductServiceTests;

public class ProductControllerTests
{
  private readonly NullLogger<ProductsController> _nullLogger;

  public ProductControllerTests()
  {
    _nullLogger = new NullLogger<ProductsController>();
  }

  [Theory]
  [InlineData(1, typeof(OkObjectResult))]
  [InlineData(0, typeof(NoContentResult))]
  public async Task GetProduct_ProductId_Product(int productId,
    Type statusCodeResponse)
  {
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProduct(productId);

    Assert.Equal(statusCodeResponse, result.Result!.GetType());
  }

  [Fact]
  public async Task
    GetProdutsPage_NameFilter_ProductsThatNameContainsSubstring()
  {
    SearchFilters nameFilter = new()
    {
      Name = "blue",
      Order = SearchFilters.SortType.MinPrice
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, nameFilter);

    var data =
      ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    foreach (var entity in data!)
      Assert.Contains(nameFilter.Name, entity.Name,
        StringComparison.InvariantCultureIgnoreCase);
  }

  [Theory]
  [InlineData(false, 1)]
  [InlineData(true, 3)]
  public async Task GetAdjacentPage_Filters_ProperPageWithItem(
    bool isPreviousPage, int expectedProductId)
  {
    const int PAGE_SIZE = 1;

    SearchFilters filters = new();
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      CategoryId = 1,
      Name = "Red Cup",
      Description = "Fairly big cup",
      Price = 20,
      Quantity = 10,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    };

    var result = await _cut
      .GetAdjacentPage(PAGE_SIZE, isPreviousPage, filters, referencedItem);

    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(expectedProductId, data.First().Id);    
  }

  [Fact]
  public async Task GetProductsPage_PriceFilter_ProductsBetweenMinAndMaxPrice()
  {
    SearchFilters priceFilter = new()
    {
      MinPrice = 30,
      MaxPrice = 40,
      Order = SearchFilters.SortType.MinPrice
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, priceFilter);

    var data =
      ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;

    foreach (var entity in data!)
    {
      Assert.True(entity.Price >= priceFilter.MinPrice);
      Assert.True(entity.Price <= priceFilter.MaxPrice);
    }
  }

  [Fact]
  public async Task GetProductsPage_QuantityFilter_ProductsWithEnoughSupply()
  {
    SearchFilters quantityFilter = new()
    {
      MinQuantity = 30,
      Order = SearchFilters.SortType.MinPrice
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, quantityFilter);

    var data =
      ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;

    foreach (var entity in data!)
      Assert.True(entity.Price >= quantityFilter.MinQuantity);
  }

  [Theory]
  [InlineData(1, 20, typeof(NoContentResult))]
  [InlineData(-5, 20, typeof(BadRequestObjectResult))]
  [InlineData(1, 250, typeof(BadRequestObjectResult))]
  public async Task GetProductsPage_InvalidPageParams_AppropriateResponse(
    int page, int pageSize, Type resultType)
  {
    var _db = new ProductDbContextFakeBuilder()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result =
      await _cut.GetProductsPage(page, pageSize, new SearchFilters
      {
        Order = SearchFilters.SortType.MinPrice
      });

    Assert.Equal(resultType, result.Result!.GetType());
  }

  [Fact]
  public async Task AddNewProduct_ValidProduct_ProductAddedToDatabase()
  {
    var p = new Product()
    {
      Name = "Sample product",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0
    };
    var URL = "/api/v1/Products/1";
    var _db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsController(_nullLogger, _db)
    {
      Url = Substitute.For<IUrlHelper>()
    };
    _cut.Url.Action().Returns(URL);

    var result = await _cut.AddNewProduct(p);

    Assert.IsType<CreatedResult>(result);
  }

  [Fact]
  public async Task
    AddNewProduct_ProductMissingRequiredField_DbUpdateException()
  {
    Product p = new()
    {
      Name = "Sample product",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0
    };
    p.Name = null;
    var _db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var e = Record.ExceptionAsync(
      async () => await _cut.AddNewProduct(p));

    Assert.IsType<DbUpdateException>(e.Result);
  }

  [Fact]
  public async Task AddNewProduct_InvalidProductCategory_DbUpdateException()
  {
    Product p = new()
    {
      Name = "Sample product",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0
    };
    p.CategoryId = -1;
    var _db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsController(_nullLogger, _db)
    {
      Url = Substitute.For<IUrlHelper>()
    };
    var result = await _cut.AddNewProduct(p);

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task UpdateProduct_ChangedProduct_ConcurrencyStampChanged()
  {
    var _db = new ProductDbContextFakeBuilder().WithCategories().WithProducts()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);
    var productResult = await _cut.GetProduct(1);
    var newProductData =
      ((productResult.Result as OkObjectResult)!.Value as Product)!;
    newProductData.Description = "Fresh and intriguing description";
    var oldStamp = newProductData.ConcurrencyStamp!;

    var result = await _cut.UpdateProduct(newProductData.Id, newProductData);

    var updatedProduct = ((result.Result as OkObjectResult)!.Value as Product)!;
    Assert.NotEqual(oldStamp, updatedProduct.ConcurrencyStamp);
  }

  [Fact]
  public async Task UpdateProduct_ProductWithNonexistentCategory_CategoryError()
  {
    var _db = new ProductDbContextFakeBuilder().WithCategories().WithProducts()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);
    var productResult = await _cut.GetProduct(1);
    var newProductData =
      ((productResult.Result as OkObjectResult)!.Value as Product)!;
    newProductData.CategoryId = 5555;

    var result = await _cut.UpdateProduct(newProductData.Id, newProductData);

    var errorString = (result.Result as NotFoundObjectResult)!.Value as string;
    Assert.Contains("category", errorString);
  }

  [Fact]
  public async Task
    UpdateProduct_OutdatedConcurrencyStamp_DbUpdateConcurrencyException()
  {
    var _db = new ProductDbContextFakeBuilder().WithCategories().WithProducts()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);
    var productResult = await _cut.GetProduct(1);
    var newProductData =
      ((productResult.Result as OkObjectResult)!.Value as Product)!;

    newProductData.ConcurrencyStamp[0] += 1;
    var ex = Record.ExceptionAsync(async () =>
      await _cut.UpdateProduct(newProductData.Id, newProductData));

    Assert.IsType<DbUpdateConcurrencyException>(ex.Result);
  }
}