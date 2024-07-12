using ProductService.Controllers.v1;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis;
using System.Net.Http.Json;
using System.Net;
using System.Drawing.Printing;

namespace ProductServiceTests;

public class ProductsCategoriesControllerTests : IClassFixture<AppFixture>
{
  private const string ApiUrl = "http://localhost/api/";
  private readonly AppFixture _app;
  private readonly HttpClient _client;
  private const int CategoryToDeleteId = 1;
  public ProductsCategoriesControllerTests(AppFixture app)
  {
	_app = app;
	_client = app.CreateClient();
  }
  [Fact]
  public async void GetProductCategory_ValidID_ProductCategory()
  {
	const int CategoryId = 2;

	var result = await _client.GetAsync($"api/v1/ProductsCategories/{CategoryId}");

    var category = await result.Content.ReadFromJsonAsync<ProductCategory>();
    Assert.IsType<ProductCategory>(category);
  }

  [Fact]
  public async void GetProductCategory_NonExistingID_NotFound()
  {
    const int CategoryId = 55555;

	var result = await _client.GetAsync($"api/v1/ProductsCategories/{CategoryId}");

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async void GetReleatedCategories_CategoryWithChildren_ReleatedCategories()
  {
    const int CategoryId = 6;

	var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CategoryId}");

	var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
    Assert.True(children!.Count > 0);
  }

  [Fact]
  public async void GetReleatedCategories_CategoryWithNoChildren_EmptyList()
  {
	const int CategoryId = 2;

	var result = await _client.GetAsync($"api/v1/ProductsCategories/children/{CategoryId}");

	var children = await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
	Assert.Empty(children!);
  }

  [Fact]
  public async void PostProductCategory_ValidNewCategory_Created201()
  {
    var newCategory = new ProductCategory()
    {
      CategoryName = "Glass",
      ParentCategory = new()
      {
        CategoryName = "Tableware",
        Id = 2,
      }
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
  public async void PostProductCategory_ExistingCategory_Conflict409()
  {
    var existingCategory = new ProductCategory()
    {
      CategoryName = "Tableware",
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
  public async void PostProductCategory_CategoryWithNonExistingParent_BadRequest400()
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
  public async void PatchProductCategory_ValidCategoryUpdate_NoContent204()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 3,
      CategoryName = "New category name",
    };
	HttpRequestMessage msg = new()
	{
	  Method = HttpMethod.Patch,
	  RequestUri = new UriBuilder($"{ApiUrl}v1/ProductsCategories/{updatedCategory.Id}").Uri,
	  Content = JsonContent.Create(updatedCategory)
	};

	var result = await _client.SendAsync(msg);

	Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }

  [Fact]
  public async void PatchProductCategory_NonExistingCategoryUpdate_NotFound404()
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
  public async void PatchProductCategory_NonExistingParentCategoryUpdate_NotFound404()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 3,
      CategoryName = "PC Parts",
      ParentCategory = new ProductCategory()
      {
        Id = 2,
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
  public async void DeleteProductCategory_ValidExistingCategory_NoContent204()
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
  public async void DeleteProductCategory_NonExistingCategory_NotFound404()
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