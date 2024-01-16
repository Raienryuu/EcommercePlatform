using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using ProductService.Controllers;
using ProductService.Models;

namespace ProductServiceTests;

public class ProductControllerTests
{

  private readonly ProductDbContextFakeBuilder _dbBuilder = new();
  private readonly ProductsController _cut;
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

    IEnumerable<Product> data = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;
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

    IEnumerable<Product> data = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;

        foreach (var entity in data!)
        {
            Assert.True(entity.Price >= priceFilter.MinPrice);
            Assert.True(entity.Price <= priceFilter.MaxPrice);
        }
    }

    [Fact]
    public async Task GetProdutsPage_QuantityFilter_ProductsWithEnoughSupply()
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

        IEnumerable<Product> data = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;

        foreach (var entity in data!)
        {
            Assert.True(entity.Price >= quantityFilter.MinQuantity);
        }
    }
}

