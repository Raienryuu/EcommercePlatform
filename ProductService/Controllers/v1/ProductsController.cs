using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Services;
using ProductService.Utility;

namespace ProductService.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController(
  ILogger<ProductsController> logger,
  ProductDbContext db,
  IProductService productService
) : ControllerBase
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
  [Route("{id:guid}")]
  [ProducesResponseType<Product>(StatusCodes.Status200OK)]
  [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProduct(Guid id)
  {
    var result = await db.Products.FindAsync(id);
    return result is not null ? Ok(result) : NotFound(($"No product found with given ID: {id}.", id));
  }

  [HttpGet]
  [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
  [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetProductsPage([FromQuery] PaginationParams filters)
  {
    var validationResult = ValidatePaginationParams(filters.PageSize, filters.PageNum);

    if (validationResult is not null)
    {
      return validationResult;
    }

    var pagination = new ProductsPagination(filters, db).GetOffsetPageQuery(
      filters.PageNum,
      filters.PageSize
    );
    var products = await pagination.ToListAsync();

    return Ok(products);
  }

  [HttpPost]
  [Route("nextPage")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetNextPage(
    [FromQuery] PaginationParams filters,
    [FromBody] Product referenceProduct
  )
  {
    if (filters.PageSize is < 1 or > 200)
    {
      return BadRequest(
        ProductsControllerHelpers.CreateErrorResponse("PageSize should be between 1 and 200")
      );
    }
    var query = new ProductsPagination(filters, db).GetNextPageQuery(filters.PageSize, referenceProduct);

    var products = await query.ToListAsync();
    return Ok(products);
  }

  [HttpPost]
  [Route("previousPage")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> GetPreviousPage(
    [FromQuery] PaginationParams filters,
    [FromBody] Product referenceProduct
  )
  {
    if (filters.PageSize is < 1 or > 200)
    {
      return BadRequest(
        ProductsControllerHelpers.CreateErrorResponse("PageSize should be between 1 and 200")
      );
    }
    var query = new ProductsPagination(filters, db).GetPreviousPageQuery(filters.PageSize, referenceProduct);

    var products = await query.ToListAsync();
    return Ok(products);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<ActionResult> AddNewProduct([FromBody] Product newProduct)
  {
    newProduct.Category = await db.ProductCategories.SingleOrDefaultAsync(c => c.Id == newProduct.CategoryId);

    if (newProduct.Category is null)
    {
      return BadRequest(ProductsControllerHelpers.CreateErrorResponse("Category not found"));
    }
    newProduct.RefreshConcurrencyStamp();
    _ = db.Products.Add(newProduct);
    _ = await db.SaveChangesAsync();

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
  [HttpPatch("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] Product updatedProduct)
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(p => p.Id == id);
    if (oldProduct is null)
    {
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("Product not found"));
    }
    if (!await DoesCategoryExists(updatedProduct.CategoryId))
    {
      return NotFound(ProductsControllerHelpers.CreateErrorResponse("Given category does not exists"));
    }
    if (updatedProduct.ConcurrencyStamp != oldProduct.ConcurrencyStamp)
    {
      return UnprocessableEntity(ProductsControllerHelpers.CreateErrorResponse("ConcurrencyStamp mismatch"));
    }
    await ProductsControllerHelpers.AssignNewValuesToProduct(db, updatedProduct, oldProduct);

    oldProduct.RefreshConcurrencyStamp();
    System.Diagnostics.Debug.WriteLine(oldProduct.ConcurrencyStamp);

    _ = await db.SaveChangesAsync();

    return Ok(oldProduct);
  }

  private async Task<bool> DoesCategoryExists(int categoryId)
  {
    var result = await db.ProductCategories.FirstOrDefaultAsync(cat => cat.Id == categoryId);
    return result is not null;
  }

  private IActionResult? ValidatePaginationParams(int pageSize, int pageNum = 1)
  {
    return pageNum < 1 || pageSize < 1 || pageSize > 200
      ? BadRequest(
        ProductsControllerHelpers.CreateErrorResponse(
          "Page and PageSize must be greater than 0 and PageSize less " + "than 200"
        )
      )
      : (IActionResult?)null;
  }

  /// <summary>
  /// Gets IEnumerable of Product for given Ids.
  /// </summary>
  /// <param name="productsIds"></param>
  /// <returns>List of products.</returns>
  /// <response code="200">List of products.</response>
  /// <response code="404">If product ids doesn't exist.</response>
  [HttpPost("batch")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IResult> GetSelectiveProducts([FromBody] List<Guid> productsIds)
  {
    if (productsIds.Count == 0)
    {
      return Results.BadRequest("No products id's were provided.");
    }

    var result = await productService.GetBatchProducts(productsIds);
    return result.StatusCode switch
    {
      200 => Results.Ok(result.Value),
      400 => Results.BadRequest(result.ErrorMessage),
      404 => Results.NotFound(result.ErrorMessage),
      _ => throw new NotImplementedException("Unexpected status code"),
    };
  }
}
