using System.Net;
using System.Text.Json;
using ProductService.Models;

namespace ProductService.Tests.Integration;

[Collection("Tests")]
public class ProductControllerTests(AppFixture appFixture) : TempFixture(appFixture)
{
  private static readonly JsonSerializerOptions s_jsonOptions = new() { PropertyNameCaseInsensitive = true };
  private const string API_URL = "http://localhost/api/";

  [Fact]
  public async Task GetProductsPage_ProductNameFilter_ProductsThatNameContainsSubstring()
  {
    PaginationParams nameFilter = new()
    {
      PageNum = 1,
      PageSize = 20,
      Name = "White",
      Order = PaginationParams.SortType.PriceAsc,
    };

    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/" + $"?Name={nameFilter.Name}&Order={nameFilter.Order}"
      ).Uri,
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    foreach (var entity in data!)
    {
      Assert.Contains(nameFilter.Name, entity.Name);
    }
  }

  [Theory]
  [InlineData("87817c15-d25f-4621-9135-2e7851b484b3", HttpStatusCode.OK)]
  [InlineData("983317d9-14fd-4126-b499-9e5186b14497", HttpStatusCode.NotFound)]
  public async Task GetProduct_ProductId_ProductOrNoContent(
    string productId,
    HttpStatusCode statusCodeResponse
  )
  {
    var productGuid = Guid.Parse(productId);
    var result = await _client.GetAsync($"api/v1/products/{productGuid}");

    Assert.Equal(statusCodeResponse, result.StatusCode);
  }

  [Fact]
  public async Task GetNextPage_PriceAscendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f40d0b49-e347-46f8-a127-b0c359c2ffce"),
      CategoryId = 1,
      Name = "White Cup",
      Description = "Fairly big cup",
      Price = 15,
      Quantity = 0,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products/nextPage?PageSize={filters.PageSize}").Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Price <= data[i].Price);
      Assert.True(referencedItem.Price <= data[i].Price);
    }
  }

  [Fact]
  public async Task GetPreviousPage_PriceAscendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products/previousPage?PageSize={filters.PageSize}").Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Price <= data[i].Price);
      Assert.True(referencedItem.Price >= data[i].Price);
    }
  }

  [Fact]
  public async Task GetNextPage_PriceDescendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { Order = PaginationParams.SortType.PriceDesc, PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/nextPage/" + $"?Order={filters.Order}&PageSize={filters.PageSize}"
      ).Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Price >= data[i].Price);
      Assert.True(referencedItem.Price >= data[i].Price);
    }
  }

  [Fact]
  public async Task GetPreviousPage_PriceDescendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { Order = PaginationParams.SortType.PriceDesc, PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/previousPage/" + $"?Order={filters.Order}&PageSize={filters.PageSize}"
      ).Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Price >= data[i].Price);
      Assert.True(referencedItem.Price <= data[i].Price);
    }
  }

  [Fact]
  public async Task GetNextPage_QuantityAscendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { Order = PaginationParams.SortType.QuantityAsc, PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/nextPage/" + $"?Order={filters.Order}&PageSize={filters.PageSize}"
      ).Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Quantity <= data[i].Quantity);
      Assert.True(referencedItem.Quantity <= data[i].Quantity);
    }
  }

  [Fact]
  public async Task GetPreviousPage_QuantityAscendingOrder_ProperPageWithItem()
  {
    PaginationParams filters = new() { Order = PaginationParams.SortType.QuantityAsc, PageSize = 2 };
    Product referencedItem = new()
    {
      Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/previousPage" + $"?Order={filters.Order}&PageSize={filters.PageSize}"
      ).Uri,
      Content = JsonContent.Create(referencedItem),
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    for (var i = 1; i < data!.Count; i++)
    {
      Assert.True(data[i - 1].Quantity <= data[i].Quantity);
      Assert.True(referencedItem.Quantity >= data[i].Quantity);
    }
  }

  [Fact]
  public async Task GetProductsPage_PriceFilter_ProductsBetweenMinAndMaxPrice()
  {
    PaginationParams priceFilter = new()
    {
      MinPrice = 30,
      MaxPrice = 40,
      PageNum = 1,
      PageSize = 20,
    };

    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/"
          + $"?MinPrice={priceFilter.MinPrice}&MaxPrice={priceFilter.MaxPrice}"
          + $"&PageNum={priceFilter.PageNum}&PageSize={priceFilter.PageSize}"
      ).Uri,
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    foreach (var entity in data!)
    {
      Assert.True(entity.Price >= priceFilter.MinPrice);
      Assert.True(entity.Price <= priceFilter.MaxPrice);
    }
  }

  [Fact]
  public async Task GetProductsPage_QuantityFilter_ProductsWithEnoughSupply()
  {
    PaginationParams quantityFilter = new()
    {
      MinQuantity = 30,
      PageNum = 1,
      PageSize = 20,
    };

    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products/"
          + $"?MinQuantity={quantityFilter.MinQuantity}"
          + $"&PageNum={quantityFilter.PageNum}&PageSize={quantityFilter.PageSize}"
      ).Uri,
    };

    var result = await _client.SendAsync(msg);

    var data = await result.Content.ReadFromJsonAsync<List<Product>>();
    foreach (var entity in data!)
    {
      Assert.True(entity.Quantity >= quantityFilter.MinQuantity);
    }
  }

  [Theory]
  [InlineData(1, 20, HttpStatusCode.OK)]
  [InlineData(-5, 20, HttpStatusCode.BadRequest)]
  [InlineData(1, 250, HttpStatusCode.BadRequest)]
  public async Task GetProductsPage_PageParams_AppropriateResponse(
    int page,
    int pageSize,
    HttpStatusCode httpResponseCode
  )
  {
    var pageFilter = new PaginationParams { PageSize = pageSize, PageNum = page };

    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(
        $"{API_URL}v1/products?PageSize={pageFilter.PageSize}&PageNum={pageFilter.PageNum}"
      ).Uri,
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(httpResponseCode, result.StatusCode);
  }

  [Fact]
  public async Task AddNewProduct_ValidProduct_ProductAddedToDatabase()
  {
    var p = new Product()
    {
      Name = "Sample product1",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0,
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(p),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
  }

  [Fact]
  public async Task AddNewProduct_ProductMissingRequiredFieldName_NameFieldIsRequiredError()
  {
    Product p = new()
    {
      Name = "Sample product2",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0,
    };
    p.Name = null!;
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(p),
    };

    var result = await _client.SendAsync(msg);

    var content = await result.Content.ReadAsStringAsync();
    Assert.Contains("Name field is required", content);
  }

  [Fact]
  public async Task AddNewProduct_InvalidProductCategory_CategoryNotFoundError()
  {
    Product p = new()
    {
      Name = "Sample product3",
      CategoryId = 1,
      Price = 15m,
      Description = "Do not buy",
      Quantity = 0,
    };
    p.CategoryId = -1;
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(p),
    };

    var result = await _client.SendAsync(msg);

    var content = await result.Content.ReadAsStringAsync();
    Assert.Contains("Category not found", content);
  }

  [Fact]
  public async Task UpdateProduct_ChangedProduct_ConcurrencyStampChanged()
  {
    var productId = Guid.Parse("87817c15-d25f-4621-9135-2e7851b484b3");
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder($"{API_URL}v1/products/{productId}").Uri,
    };
    var productResult = await _client.SendAsync(msg);
    var newProduct = await productResult.Content.ReadFromJsonAsync<Product>();
    newProduct!.Description = "Fresh and intriguing description";
    var oldStamp = newProduct.ConcurrencyStamp;
    msg = new()
    {
      Method = HttpMethod.Put,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(newProduct),
    };

    var result = await _client.SendAsync(msg);
    // ReadFromStringAsync() was unable to correctly deserialize object, ConcurrencyStamp was default instead of actual value
    var updatedProduct = await result.Content.ReadAsStringAsync();
    var productInstance = JsonSerializer.Deserialize<Product>(updatedProduct, options: s_jsonOptions);
    Assert.NotEqual(oldStamp, productInstance!.ConcurrencyStamp);
  }

  [Fact]
  public async Task UpdateProduct_OutdatedConcurrencyStamp_ConcurrencyStampMismatchError()
  {
    var productId = Guid.Parse("87817c15-d25f-4621-9135-2e7851b484b3");
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder($"{API_URL}v1/products/{productId}").Uri,
    };
    var productResult = await _client.SendAsync(msg);
    var product = await productResult.Content.ReadFromJsonAsync<Product>();

    msg = new()
    {
      Method = HttpMethod.Put,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(product),
    };
    _ = await _client.SendAsync(msg);
    msg = new()
    {
      Method = HttpMethod.Put,
      RequestUri = new UriBuilder($"{API_URL}v1/products").Uri,
      Content = JsonContent.Create(product),
    };

    var result = await _client.SendAsync(msg);

    var content = await result.Content.ReadAsStringAsync();
    Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
    Assert.Contains("ConcurrencyStamp", content);
  }

  [Fact]
  public async Task GetProductsList_ListOfExistingProductsIds_ProductsList()
  {
    var productsIds = new[]
    {
      Guid.Parse("87817c15-d25f-4621-9135-2e7851b484b3"),
      Guid.Parse("22ea176b-ea99-445f-97b3-c1afa5585562"),
      Guid.Parse("f12e47a0-82f9-4231-abce-63280e7d3d99"),
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{API_URL}v1/products/batch").Uri,
      Content = JsonContent.Create(productsIds),
    };

    var result = await _client.SendAsync(msg);

    var products = await result.Content.ReadFromJsonAsync<List<Product>>();
    Assert.Equal(productsIds.Length, productsIds.Length);
    products!.ForEach(x => Assert.Contains(x.Id, productsIds));
  }
}
