using Microsoft.AspNetCore.Mvc;
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
  [InlineData(0, typeof(NotFoundObjectResult))]
  public async Task GetProduct_ProductId_ProductOrNoContent(int productId,
    Type statusCodeResponse)
  {
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProduct(productId);

    Assert.Equal(statusCodeResponse, result.GetType());
  }

  [Fact]
  public async Task
    GetProdutsPage_ProductNameFilter_ProductsThatNameContainsSubstring()
  {
    SearchFilters nameFilter = new()
    {
      Name = "White",
      Order = SearchFilters.SortType.PriceAsc
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, nameFilter);

    var data =
      ((result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    foreach (var entity in data!)
      Assert.Contains(nameFilter.Name, entity.Name);
  }

  [Fact]
  public async Task GetNextPage_PriceAscendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;

    const int EXPECTED_ID = 3;

    SearchFilters filters = new();
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetNextPage(PAGE_SIZE, filters, referencedItem);

    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

  [Fact]
  public async Task GetPreviousPage_PriceAscendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;

    const int EXPECTED_ID = 5;

    SearchFilters filters = new();
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetPreviousPage(PAGE_SIZE, filters, referencedItem);

    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

  [Fact]
  public async Task GetNextPage_PriceDescendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;

    const int EXPECTED_ID = 5;

    SearchFilters filters = new(){
      Order = SearchFilters.SortType.PriceDesc
    };
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetNextPage(PAGE_SIZE, filters, referencedItem);
    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

[Fact]
  public async Task GetPreviousPage_PriceDescendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;
    const int EXPECTED_ID = 3;

    SearchFilters filters = new(){
      Order = SearchFilters.SortType.PriceDesc
    };
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetPreviousPage(PAGE_SIZE, filters, referencedItem);
    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

  [Fact]
  public async Task GetNextPage_QuantityAscendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;

    const int EXPECTED_ID = 1;

    SearchFilters filters = new(){
      Order = SearchFilters.SortType.QuantityAsc
    };
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetNextPage(PAGE_SIZE, filters, referencedItem);
    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

[Fact]
  public async Task GetPreviousPage_QuantityAscendingOrder_ProperPageWithItem()
  {
    const int PAGE_SIZE = 1;
    const int EXPECTED_ID = 2;

    SearchFilters filters = new(){
      Order = SearchFilters.SortType.QuantityAsc
    };
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);
    Product referencedItem = new()
    {
      Id = 4,
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };

    var result = await _cut
      .GetPreviousPage(PAGE_SIZE, filters, referencedItem);
    var data = ((result.Result as OkObjectResult)!.Value as IEnumerable<Product>)!;
    Assert.Equal(EXPECTED_ID, data.First().Id);
  }

  [Fact]
  public async Task GetProductsPage_PriceFilter_ProductsBetweenMinAndMaxPrice()
  {
    SearchFilters priceFilter = new()
    {
      MinPrice = 30,
      MaxPrice = 40,
      Order = SearchFilters.SortType.PriceAsc
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, priceFilter);

    var data =
      ((result as OkObjectResult)!.Value as IEnumerable<Product>)!;

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
      Order = SearchFilters.SortType.PriceAsc
    };
    var PAGE = 1;
    var PAGE_SIZE = 20;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, quantityFilter);

    var data =
      ((result as OkObjectResult)!.Value as IEnumerable<Product>)!;

    foreach (var entity in data!)
      Assert.True(entity.Quantity >= quantityFilter.MinQuantity);
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task GetProductsPage_OrderByPriceFilter_ProductsOrderedByPrice
    (bool isPriceAscending)
  {
    SearchFilters quantityFilter = new()
    {
      Order = isPriceAscending ? SearchFilters.SortType.PriceAsc : SearchFilters.SortType.PriceDesc
    };
    var PAGE = 1;
    var PAGE_SIZE = 5;
    var _db = new ProductDbContextFakeBuilder()
      .WithProducts().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result = await _cut.GetProductsPage(
      PAGE, PAGE_SIZE, quantityFilter);

    var data =
      ((result as OkObjectResult)!.Value as IEnumerable<Product>)!;

    Product previous_entity = data.First();

    foreach (var entity in data!)
    {
      if (isPriceAscending)
      {
        Assert.True(entity.Price >= previous_entity.Price);
      }
      else
      {
        Assert.True(entity.Price <= previous_entity.Price);
      }
      previous_entity = entity;
    }
  }

  [Theory]
  [InlineData(1, 20, typeof(OkObjectResult))]
  [InlineData(-5, 20, typeof(BadRequestObjectResult))]
  [InlineData(1, 250, typeof(BadRequestObjectResult))]
  public async Task GetProductsPage_PageParams_AppropriateResponse(
    int page, int pageSize, Type resultType)
  {
    var _db = new ProductDbContextFakeBuilder()
      .Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var result =
      await _cut.GetProductsPage(page, pageSize, new SearchFilters
      {
        Order = SearchFilters.SortType.PriceAsc
      });

    Assert.Equal(resultType, result.GetType());
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

    Assert.IsType<CreatedAtActionResult>(result);
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
    p.Name = null!;
    var _db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsController(_nullLogger, _db);

    var e = await Record.ExceptionAsync(
      async () => await _cut.AddNewProduct(p));

    Assert.IsType<DbUpdateException>(e);
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
      ((productResult as OkObjectResult)!.Value as Product)!;
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
      ((productResult as OkObjectResult)!.Value as Product)!;
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
      ((productResult as OkObjectResult)!.Value as Product)!;

    newProductData.ConcurrencyStamp![0] += 1;
    var ex = await Record.ExceptionAsync(async () =>
      await _cut.UpdateProduct(newProductData.Id, newProductData));

    Assert.IsType<DbUpdateConcurrencyException>(ex);
  }
}