using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility;

namespace ProductService.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController(
  ILogger<ProductsController> logger,
  ProductDbContext db)
  : ControllerBase
{
  private readonly ILogger<ProductsController> _logger = logger;

  /// <summary>
  /// Gets product with a given <paramref name="id"/>.
  /// </summary>
  /// <param name="id">Identifier of a product to find.</param>
  /// <returns><ref name="result">product</ref> object</returns>
  /// <response code="200">Found <ref name="Product">product</ref></response>
  /// <response code="404">If product doesn't exists</response>
  [HttpGet]
  [Route("{id}")]
  [ProducesResponseType<Product>(StatusCodes.Status200OK)]
  [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProduct(int id)
  {
    var result = await db.Products.FindAsync(id);
    if (result is not null) return Ok(result);
    return NotFound(($"No product found with given ID: {id}.", id));
  }

  [HttpGet]
  [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
  [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetProductsPage(
  [FromQuery] PaginationParams filters)
  {
    var validationResult = ValidatePaginationParams(filters.PageSize, filters.PageNum);

    if (validationResult is not null)
    {
      return validationResult;
    }

    var pagination = new ProductsPagination(filters, db)
      .GetOffsetPageQuery(filters.PageNum, filters.PageSize);
    var products = await pagination.ToListAsync();

    return Ok(products);
  }

  [HttpPost]
  [Route("nextPage")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetNextPage(
  [FromQuery] PaginationParams filters,
  [FromBody] Product referenceProduct)
  {

    if (filters.PageSize < 1 || filters.PageSize > 200)
      return BadRequest(ProductsControllerHelpers.CreateErrorResponse(
	  "PageSize should be between 1 and 200"));

	var query = new ProductsPagination(filters, db)
      .GetNextPageQuery(filters.PageSize, referenceProduct);

    var s = query.ToQueryString();
    Console.WriteLine(s);

    var products = await query.ToListAsync();
    return Ok(products);
  }

  [HttpPost]
  [Route("previousPage")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetPreviousPage(
  [FromQuery] PaginationParams filters,
  [FromBody] Product referenceProduct)
  {
    if (filters.PageSize < 1 || filters.PageSize > 200)
      return BadRequest(ProductsControllerHelpers.CreateErrorResponse(
      "PageSize should be between 1 and 200"));

    var query = new ProductsPagination(filters, db)
      .GetPreviousPageQuery(filters.PageSize, referenceProduct);

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
      return BadRequest(ProductsControllerHelpers.CreateErrorResponse("Category not found"));

    newProduct.RefreshConcurrencyStamp();
    db.Products.Add(newProduct);
    await db.SaveChangesAsync();

    return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
  }

  /// <summary>
  /// Updates product with give data.
  /// </summary>
  /// <remarks>
  /// Requires <see cref="Product.ConcurrencyStamp"/>s to be equal.
  /// </remarks>
  /// <param name="id"> of product to be modified</param>
  /// <param name="updatedProduct"> object with new values</param>
  /// <returns>Updated product.</returns>
  /// <response code="200">Product with updated values.</response>
  /// <response code="404">If product id doesn't exist.</response>
  /// <response code="422">If ConcurrencyStamps don't match.</response>
  [HttpPatch("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<Product>> UpdateProduct(int id,
  [FromBody] Product updatedProduct)
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(p => p.Id == id);
    if (oldProduct is null)
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("Product not found"));

    if (!await DoesCategoryExists(updatedProduct.CategoryId))
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("Given category does not exists"));

    if (updatedProduct.ConcurrencyStamp != oldProduct.ConcurrencyStamp)
      return UnprocessableEntity(
      ProductsControllerHelpers
      .CreateErrorResponse("ConcurrencyStamp mismatch"));

    await ProductsControllerHelpers
      .AssignNewValuesToProduct(db, updatedProduct, oldProduct);

    oldProduct.RefreshConcurrencyStamp();
    System.Diagnostics.Debug.WriteLine(oldProduct.ConcurrencyStamp);

    await db.SaveChangesAsync();

    return Ok(oldProduct);
  }

  private async Task<bool> DoesCategoryExists(int categoryId)
  {
    var result = await db.ProductCategories.FirstOrDefaultAsync(
      cat => cat.Id == categoryId);
    return result is not null;
  }

  private IActionResult? ValidatePaginationParams(int pageSize, int pageNum = 1)
  {
    if (pageNum < 1 || pageSize < 1 || pageSize > 200)
      return BadRequest(ProductsControllerHelpers.CreateErrorResponse(
      "Page and PageSize must be greater than 0 and PageSize less " +
      "than 200"));
    return null;
  }

  /// <summary>
  /// Gets IEnumerable of Product for given Ids.
  /// </summary>
  /// <param name="productsIds"></param>
  /// <returns>Updated product.</returns>
  /// <response code="200">Product with updated values.</response>
  /// <response code="404">If product id doesn't exists.</response>
  [HttpGet("batch")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<IEnumerable<Product>>> GetSelectiveProducts([FromBody] int[] productsIds)
  {
    if (productsIds is null)
    {
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("No products Id were passed."));
    }

    var products = await db.Products.Where(x => productsIds.Contains(x.Id)).ToListAsync();

    if (products.Count != productsIds.Length)
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("Some products Id were not found"));

    return Ok(products);
  }
}