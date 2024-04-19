using ProductService.Controllers.v1;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using Microsoft.IdentityModel.Tokens;

namespace ProductServiceTests;

public class ProductsCategoriesControllerTests
{

  [Fact]
  public async void GetProductCategory_ValidID_ProductCategory()
  {
    const int CATEGORYID = 1;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.GetProductCategory(CATEGORYID);

    Assert.NotNull(result);
  }

  [Fact]
  public async void GetProductCategory_NonExistingID_NotFound()
  {
    const int CATEGORYID = -1;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.GetProductCategory(CATEGORYID);

    Assert.Equal(typeof(NotFoundObjectResult), result.Result!.GetType());
  }

  [Fact]
  public async void GetReleatedCategories_CategoryWithChildren_ReleatedCategories()
  {
    const int CATEGORYID = 3;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.GetChildrenCategories(CATEGORYID);

    Assert.True(
((result.Result as OkObjectResult)!
            .Value as IEnumerable<ProductCategory>)!.Any());
  }

  [Fact]
  public async void GetReleatedCategories_CategoryWithNoChildren_EmptyList()
  {
    const int CATEGORYID = 2;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.GetChildrenCategories(CATEGORYID);

    Assert.True(
((result.Result as OkObjectResult)!
            .Value as IEnumerable<ProductCategory>)!.IsNullOrEmpty());
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
        Id = 1,
      }
    };
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PostProductCategory(newCategory);

    Assert.Equal(201, (result.Result as CreatedAtActionResult)!.StatusCode);
  }

  [Fact]
  public async void PostProductCategory_ExistingCategory_Conflict409()
  {
    var existingCategory = new ProductCategory()
    {
      CategoryName = "Tableware",
    };
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PostProductCategory(existingCategory);

    Assert.Equal(409, (result.Result as ConflictObjectResult)!.StatusCode);
  }

  [Fact]
  public async void PostProductCategory_CategoryWithNonExistingParent_BadRequest400()
  {
    var newCategory = new ProductCategory()
    {
      CategoryName = "Laptops",
      ParentCategory = new() { Id = 5, CategoryName = "Electronics" }
    };
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PostProductCategory(newCategory);

    Assert.Equal(400, (result.Result as BadRequestObjectResult)!.StatusCode);
  }

  [Fact]
  public async void PatchProductCategory_ValidCategoryUpdate_NoContent204()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = 3,
      CategoryName = "Electronics",
    };
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PatchProductCategory(
      updatedCategory.Id, updatedCategory);

    Assert.Equal(204, (result as NoContentResult)!.StatusCode);
  }

  [Fact]
  public async void PatchProductCategory_NonExistingCategoryUpdate_NotFound404()
  {
    var updatedCategory = new ProductCategory()
    {
      Id = -1,
      CategoryName = "PC Parts",
    };
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PatchProductCategory(
      updatedCategory.Id, updatedCategory);

    Assert.Equal(404, (result as NotFoundObjectResult)!.StatusCode);
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
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.PatchProductCategory(
      updatedCategory.Id, updatedCategory);

    Assert.Equal(400, (result as BadRequestObjectResult)!.StatusCode);
  }

  [Fact]
  public async void DeleteProductCategory_ValidExistingCategory_NoContent204()
  {
    const int CATEGORYTODELETE = 3;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.DeleteProductCategory(CATEGORYTODELETE);

    Assert.Equal(204, (result as NoContentResult)!.StatusCode);
  }

  [Fact]
  public async void DeleteProductCategory_NonExistingCategory_NotFound404()
  {
    const int CATEGORYTODELETE = -1;
    var db = new ProductDbContextFakeBuilder().WithCategories().Build();
    var _cut = new ProductsCategoriesController(db);

    var result = await _cut.DeleteProductCategory(CATEGORYTODELETE);

    Assert.Equal(404, (result as NotFoundObjectResult)!.StatusCode);
  }



}