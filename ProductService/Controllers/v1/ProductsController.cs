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
  [Route("{pageNum}/{pageSize}")]
  [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
  [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetProductsPage(
	int pageNum, int pageSize, [FromQuery] SearchFilters filters)
  {
	var validationResult = ValidatePaginationParams(pageSize, pageNum);

	if (validationResult is not null)
	{
	  return validationResult;
	}

	var pagination = new ProductsPagination(filters, db)
	  .GetOffsetPageQuery(pageNum, pageSize);
	var products = await pagination.ToListAsync();

	if (products.Count == 0) return Ok("No products found on given page.");
	return Ok(products);
  }

  [HttpGet]
  [Route("nextPage/{pageSize}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetNextPage(
	int pageSize,
	[FromQuery] SearchFilters filters,
	[FromBody] Product product)
  {
	if (pageSize < 1 || pageSize > 200)
	  return BadRequest(CreateErrorResponse(
		"PageSize greater than 1, and PageSize less " +
		"than 200"));

	var query = new ProductsPagination(filters, db)
	  .GetNextPageQuery(pageSize, product);

	var s = query.ToQueryString();
	Console.WriteLine(s);

	var products = await query.ToListAsync();
	return Ok(products);
  }

  [HttpGet]
  [Route("previousPage/{pageSize}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetPreviousPage(
	int pageSize,
	[FromQuery] SearchFilters filters,
	[FromBody] Product product)
  {
	if (pageSize < 1 || pageSize > 200)
	  return BadRequest(CreateErrorResponse(
		"PageSize greater than 1, and PageSize less " +
		"than 200"));

	var query = new ProductsPagination(filters, db)
	  .GetPreviousPageQuery(pageSize, product);

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

	return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
  }

  /// <summary>
  /// Updates product.
  /// </summary>
  /// <remarks>
  /// Requires ConcurrencyStamps to match.
  /// </remarks>
  /// <param name="id"></param>
  /// <param name="updatedProduct"></param>
  /// <returns>Updated product with given ID.</returns>
  /// <response code="200">Product with updated values.</response>
  /// <response code="404">If product id doesn't exists.</response>
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
	  return NotFound(CreateErrorResponse("Product not found"));

	if (!await DoesCategoryExists(updatedProduct.CategoryId))
	  return NotFound(CreateErrorResponse("Given category does not exists"));

	if (oldProduct.ConcurrencyStamp is null ||
		updatedProduct.ConcurrencyStamp is null ||
		IsConcurrencyStampEqual(updatedProduct, oldProduct))
	  return UnprocessableEntity(
		CreateErrorResponse("ConcurrencyStamp mismatch"));

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

	static bool IsConcurrencyStampEqual(Product updatedProduct, Product oldProduct)
	{
	  for (int i = 0; i < oldProduct.ConcurrencyStamp!.Length; i++)
	  {
		if (oldProduct.ConcurrencyStamp[i] != updatedProduct.ConcurrencyStamp[i])
		  return false;
	  }
	  return true;
	}
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

  private IActionResult ValidatePaginationParams(int pageSize, int pageNum = 1)
  {
	if (pageNum < 1 || pageSize < 1 || pageSize > 200)
	  return BadRequest(CreateErrorResponse(
		"Page and PageSize must be greater than 0 and PageSize less " +
		"than 200"));
	return null!;
  }
}