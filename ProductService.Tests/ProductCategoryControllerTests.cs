using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductService.Models;
using ProductService.Tests.Fakes;
using System.Net;
using Testcontainers.MsSql;

namespace ProductService.Tests;

[Collection("Tests")]
public class ProductsCategoriesControllerTests(AppFixture fixture) : TempFixture(fixture)
{
  private const string ApiUrl = "http://localhost/api/";
  private const int CategoryToDeleteId = 3;


  [Fact]
  public async Task GetProductCategory_ValidID_ProductCategory()
  {
    const int CategoryId = 2;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/{CategoryId}");

    var category = await result.Content.ReadFromJsonAsync<ProductCategory>();
    Assert.IsType<ProductCategory>(category);
  }

  [Fact]
  public async Task GetProductCategory_NonExistingID_NotFound()
  {
    const int CategoryId = 55555;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/{CategoryId}");

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async Task GetReleatedCategories_CategoryWithChildren_ReleatedCategories()
  {
    const int CategoryId = 1;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CategoryId}");

    var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
    Assert.True(children!.Count > 0);
  }

  [Fact]
  public async Task GetReleatedCategories_CategoryWithNoChildren_EmptyList()
  {
    const int CategoryId = 2;

    var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CategoryId}");

    var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
    Assert.Empty(children!);
  }

  [Fact]
  public async Task PostProductCategory_ValidNewCategory_Created201()
  {
    var newCategory = new ProductCategory()
    {
      CategoryName = "Glass"
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories").Uri,
      Content = JsonContent.Create(newCategory)
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
  }

  [Fact]
  public async Task PostProductCategory_ExistingCategory_Conflict409()
  {
    var existingCategory = new ProductCategory()
    {
      CategoryName = "Sample category",
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories").Uri,
      Content = JsonContent.Create(existingCategory)
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
  }

  [Fact]
  public async Task PostProductCategory_CategoryWithNonExistingParent_BadRequest400()
  {
    var newCategory = new ProductCategory()
    {
      CategoryName = "Newest Instant Noodles",
      ParentCategory = new() { Id = 555, CategoryName = "Food" }
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories").Uri,
      Content = JsonContent.Create(newCategory)
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task PatchProductCategory_ValidCategoryUpdate_NoContent204()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 2,
      CategoryName = "TVs",
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory)
    };

    var result = await _client.SendAsync(msg);
    var cont = await result.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine(cont);
    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }

  [Fact]
  public async Task PatchProductCategory_NonExistingCategoryUpdate_NotFound404()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 66666,
      CategoryName = "PC Parts",
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory)
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
      ParentCategory = new ProductCategory()
      {
        Id = 6666,
        CategoryName = "Electronics",
      }
    };
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Patch,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{updatedCategory.Id}").Uri,
      Content = JsonContent.Create(updatedCategory)
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
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{CategoryToDeleteId}").Uri,
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }

  [Fact]
  public async Task DeleteProductCategory_NonExistingCategory_NotFound404()
  {
    const int CategoryId = 55555;
    HttpRequestMessage msg = new()
    {
      Method = HttpMethod.Delete,
      RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{CategoryId}").Uri,
    };

    var result = await _client.SendAsync(msg);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }



}