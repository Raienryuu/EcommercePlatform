using System.Net;
using ProductService.Models;

namespace ProductService.Tests.Integration;

[Collection("Tests")]
public class ProductsCategoriesControllerTests(AppFixture fixture) : TempFixture(fixture)
{
  private const string API_URL = "http://localhost/api/v1/ProductsCategories/";
  private const int CATEGORY_TO_DELETE_ID = 3;

  [Fact]
  public async Task GetProductCategory_ValidID_ProductCategory()
  {
    const int CATEGORYID = 2;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/{CATEGORYID}");

    var category = await result.Content.ReadFromJsonAsync<ProductCategory>();
    _ = Assert.IsType<ProductCategory>(category);
  }

  [Fact]
  public async Task GetProductCategory_NonExistingID_NotFound()
  {
    const int CATEGORYID = 55555;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/{CATEGORYID}");

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async Task GetReleatedCategories_CategoryWithChildren_ReleatedCategories()
  {
    const int CATEGORYID = 6;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CATEGORYID}");

    var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
    Assert.True(children!.Count > 0);
  }

  [Fact]
  public async Task GetReleatedCategories_CategoryWithNoChildren_EmptyList()
  {
    const int CATEGORYID = 2;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CATEGORYID}");

    var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
    Assert.Empty(children!);
  }

  [Fact]
  public async Task AddProductCategory_ValidNewCategory_Created201()
  {
    var newCategory = new ProductCategory() { CategoryName = "Glass" };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(API_URL).Uri,
      Content = JsonContent.Create(newCategory),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
  }

  [Fact]
  public async Task AddProductCategory_ExistingCategory_Conflict409()
  {
    var existingCategory = new ProductCategory() { CategoryName = "Sample category" };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(API_URL).Uri,
      Content = JsonContent.Create(existingCategory),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
  }

  [Fact]
  public async Task AddProductCategory_CategoryWithNonExistingParent_BadRequest400()
  {
    var newCategory = new ProductCategory()
    {
      CategoryName = "Newest Instant Noodles",
      ParentCategory = new() { Id = 555, CategoryName = "Food" },
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(API_URL).Uri,
      Content = JsonContent.Create(newCategory),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task PatchProductCategory_ValidCategoryUpdate_NoContent204()
  {
    var updatedCategory = new ProductCategory() { Id = 2, CategoryName = "TVs" };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{API_URL}{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory),
    };

    var result = await _client.SendAsync(msg);
    var cont = await result.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine(cont);
    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }

  [Fact]
  public async Task PatchProductCategory_NonExistingCategoryUpdate_NotFound404()
  {
    var updatedCategory = new ProductCategory() { Id = 66666, CategoryName = "PC Parts" };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{API_URL}{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async Task PatchProductCategory_NonExistingParentCategoryUpdate_NotFound404()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 2,
      CategoryName = "Sample category",
      ParentCategory = new ProductCategory() { Id = 6666, CategoryName = "Electronics" },
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{API_URL}{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory),
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async Task DeleteProductCategory_ValidExistingCategory_NoContent204()
  {
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Delete,
      RequestUri = new UriBuilder($"{API_URL}{CATEGORY_TO_DELETE_ID}").Uri,
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }

  [Fact]
  public async Task DeleteProductCategory_NonExistingCategory_NotFound404()
  {
    const int CATEGORYID = 55555;
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Delete,
      RequestUri = new UriBuilder($"{API_URL}{CATEGORYID}").Uri,
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }
}
