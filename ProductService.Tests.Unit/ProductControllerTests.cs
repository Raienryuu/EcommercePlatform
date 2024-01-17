using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ProductService.Controllers;
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
  public async Task GetProduct_ProductId_Product(int productId, Type statusCodeResponse)
  {
    var _db = new ProductDbContextFakeBuilder()
    .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProduct(productId);

    Assert.Equal(statusCodeResponse, result.Result!.GetType());
  }

  [Fact]
  public async Task GetProdutsPage_NameFilter_ProductsThatNameContainsSubstring()
  {
    SearchFilters nameFilter = new (){
        Name = "blue"
    };
    int PAGE = 1;
    int PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
    .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, nameFilter);

    IEnumerable<Product> data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    foreach (var entity in data!)
    {
      Assert.Contains(nameFilter.Name, entity.Name, StringComparison.InvariantCultureIgnoreCase);
    }
  }

    [Fact]
    public async Task GetProdutsPage_PriceFilter_ProductsBetweenMinAndMaxPrice()
    {
        SearchFilters priceFilter = new()
        {
            MinPrice = 30,
            MaxPrice = 40
        };
        int PAGE = 1;
        int PAGE_SIZE = 20;
        var _db = new ProductDbContextFakeBuilder()
        .WithProducts().Build();
        var _cut = new ProductsController(_nullLogger, _db);

        var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, priceFilter);

    IEnumerable<Product> data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;

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
            MinQuantity = 30
        };
        int PAGE = 1;
        int PAGE_SIZE = 20;
        var _db = new ProductDbContextFakeBuilder()
        .WithProducts().Build();
        var _cut = new ProductsController(_nullLogger, _db);

        var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, quantityFilter);

        IEnumerable<Product> data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;

        foreach (var entity in data!)
        {
            Assert.True(entity.Price >= quantityFilter.MinQuantity);
        }
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

      var result = await _cut.GetProductsPage(page, pageSize, new SearchFilters{});

      Assert.Equal(resultType, result.Result!.GetType());
    }

    [Fact]
    public async Task AddNewProduct_ValidProduct_ProductAddedToDatabase()
    {
      Product p = new Product(){
        Name = "Sample product",
        CategoryId = 1,
        Price = 15m,
        Description = "Do not buy",
        Quantity = 0
      };
      string URL = "/api/v1/Products/1";
      var _db = new ProductDbContextFakeBuilder().
        WithCategories().Build();
        var _cut = new ProductsController(_nullLogger, _db)
        {
            Url = Substitute.For<IUrlHelper>()
        };
        _cut.Url.Action().Returns(URL);

      var result = await _cut.AddNewProduct(p);

      Assert.Equal(typeof(CreatedResult), result.GetType());

    }
}

