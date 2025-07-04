using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Controllers.V1
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class ProductsCategoriesController(IProductCategoryService productCategoryService) : ControllerBase
  {
    private readonly IProductCategoryService _productCategoryService = productCategoryService;

    // GET: api/ProductsCategories
    [HttpGet]
    public async Task<ActionResult<List<ProductCategory>>> GetProductCategories()
    {
      var result = await _productCategoryService.GetProductCategories();

      if (result.IsSuccess)
      {
        return Ok(result.Value);
      }
      else
      {
        return StatusCode(result.StatusCode, result.ErrorMessage);
      }
    }

    // GET: api/ProductsCategories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
    {
      var result = await _productCategoryService.GetProductCategory(id);

      if (result.IsSuccess)
      {
        return Ok(result.Value);
      }
      else
      {
        return StatusCode(result.StatusCode, result.ErrorMessage);
      }
    }

    // GET: api/ProductsCategories/children/5
    [HttpGet("children/{id}")]
    public async Task<ActionResult<List<ProductCategory>>> GetChildrenCategories(int id)
    {
      var result = await _productCategoryService.GetChildrenCategories(id);

      if (result.IsSuccess)
      {
        return Ok(result.Value);
      }
      else
      {
        return StatusCode(result.StatusCode, result.ErrorMessage);
      }
    }

    // PATCH: api/ProductsCategories/5
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchProductCategory(int id, [FromBody] ProductCategory productCategory)
    {
      var result = await _productCategoryService.UpdateProductCategory(id, productCategory);

      if (result.IsSuccess)
      {
        return NoContent();
      }
      else
      {
        return StatusCode(result.StatusCode, result.ErrorMessage);
      }
    }

    // POST: api/ProductsCategories
    [HttpPost]
    public async Task<ActionResult<ProductCategory>> AddProductCategory(
      [FromBody] ProductCategory productCategory
    )
    {
      var result = await _productCategoryService.CreateProductCategory(productCategory);

      if (result.IsSuccess)
      {
        return CreatedAtAction("GetProductCategory", new { id = result.Value?.Id }, result.Value);
      }
      else
      {
        return Problem(result.ErrorMessage, statusCode: result.StatusCode);
      }
    }

    // DELETE: api/ProductsCategories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductCategory(int id)
    {
      var result = await _productCategoryService.DeleteProductCategory(id);

      if (result.IsSuccess)
      {
        return NoContent();
      }
      else
      {
        return StatusCode(result.StatusCode, result.ErrorMessage);
      }
    }
  }
}
