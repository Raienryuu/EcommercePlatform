using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility;

namespace ProductService.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class ProductsController(
  ILogger<ProductsController> logger,
  ProductDbContext db)
  : ControllerBase
{
  private readonly ILogger<ProductsController> _logger = logger;

  [HttpGet]
  [Route("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult<Product>> GetProduct(int id)
  {
    var result = await db.Products.SingleOrDefaultAsync(p => p.Id == id);
    if (result is not null) return Ok(result);
    return NoContent();
  }

  [HttpGet]
  [Route("{pageNum}/{pageSize}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult<IEnumerable<Product>>> GetProductsPage(
    int pageNum, int pageSize, [FromQuery] SearchFilters filters)
  {
    if (pageNum < 1 || pageSize < 1 || pageSize > 200)
      return BadRequest(CreateErrorResponse(
        "Page and PageSize must be greater than 0 and PageSize less " +
        "than 200"));

    var pagination = new ProductsPagination(filters, db)
      .GetOffsetPageQuery(pageNum, pageSize);
    var products = await pagination.ToListAsync();

    if (products.Count == 0) return NoContent();

    return Ok(products);
  }

  [HttpGet]
  [Route("adjacentPage/{pageSize}/{isPreviousPage}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult<IEnumerable<Product>>> GetAdjacentPage(
    int pageSize, bool isPreviousPage,
    [FromQuery] SearchFilters currentValues,
    [FromBody] Product product)
  {
    if (pageSize < 1 || pageSize > 200)
      return BadRequest(CreateErrorResponse(
        "PageSize greater than 1, and PageSize less " +
        "than 200"));

    var query = new ProductsPagination(currentValues, db)
      .GetAdjacentPageQuery(pageSize, isPreviousPage, product);

    var s = query.ToQueryString();
    Console.WriteLine(s);

    var products = await query.ToListAsync();
    return Ok(products);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<ActionResult> AddNewProduct([FromBody] Product newProduct)
  {
    newProduct.Category = await db.ProductCategories
      .SingleOrDefaultAsync(c => c.Id == newProduct.CategoryId);

    if (newProduct.Category is null)
      return BadRequest(CreateErrorResponse("Category not found"));

    newProduct.RefreshConcurrencyStamp();
    db.Products.Add(newProduct);
    await db.SaveChangesAsync();

    var createdUri = Url.Action(
      "GetProduct",
      "Products",
      new { id = newProduct.Id });

    return base.Created(createdUri!, newProduct);
  }


  [HttpPatch("{id}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<Product>> UpdateProduct(int id,
    [FromBody] Product updatedProduct)
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(p => p.Id == id);
    if (oldProduct is null)
      return NotFound(CreateErrorResponse("Product not found"));

    if (!DoesCategoryExists(updatedProduct.CategoryId).Result)
      return NotFound(CreateErrorResponse("Given category does not exists"));

    if (oldProduct.ConcurrencyStamp is not null &&
        updatedProduct.ConcurrencyStamp is not null &&
        oldProduct.ConcurrencyStamp.SequenceEqual(updatedProduct
          .ConcurrencyStamp))
    {
      oldProduct.Price = updatedProduct.Price;
      oldProduct.Quantity = updatedProduct.Quantity;
      oldProduct.Name = updatedProduct.Name;
      oldProduct.Description = updatedProduct.Description;
      oldProduct.CategoryId = updatedProduct.CategoryId;
      oldProduct.Category = await db.ProductCategories
        .FirstAsync(cat => cat.Id == updatedProduct.CategoryId);

      oldProduct.RefreshConcurrencyStamp();
      await db.SaveChangesAsync();

      return Ok(oldProduct);
    }

    //testign testing testing testing testnig


    return UnprocessableEntity(
      CreateErrorResponse("ConcurrencyStamp mismatch"));
  }

  private static string CreateErrorResponse(string message)
  {
    return $"{{\"type\": \"error\", \"message\": \"{message}\"}}";
  }

  private async Task<bool> DoesCategoryExists(int categoryId)
  {
    var result = await db.ProductCategories.FirstOrDefaultAsync(
      cat => cat.Id == categoryId);
    return result is not null;
  }
}