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

    var products = await new ProductsPagination(filters, db)
      .GetOffsetPageQuery(filters.PageNum, filters.PageSize)
      .ToListAsync();

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
      return BadRequest("PageSize should be between 1 and 200");
    }
    var products = await new ProductsPagination(filters, db)
      .GetNextPageQuery(filters.PageSize, referenceProduct)
      .ToListAsync();

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
      return BadRequest("PageSize should be between 1 and 200");
    }
    var products = await new ProductsPagination(filters, db)
      .GetPreviousPageQuery(filters.PageSize, referenceProduct)
      .ToListAsync();

    return Ok(products);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<ActionResult> AddNewProduct([FromBody] Product newProduct)
  {
    var createdProductResult = await productService.AddProduct(newProduct);

    return createdProductResult.IsSuccess switch
    {
      true => Ok(createdProductResult.Value),
      false => Problem(createdProductResult.ErrorMessage, null, createdProductResult.StatusCode),
    };
  }

  /// <summary>
  /// Updates product with given data.
  /// </summary>
  /// <remarks>
  /// Requires <see cref="Product.ConcurrencyStamp"/>s to be equal.
  /// </remarks>
  /// <param name="updatedProduct"> object with new values</param>
  /// <returns>Updated product.</returns>
  /// <response code="200">Product with updated values.</response>
  /// <response code="404">If product id doesn't exist.</response>
  /// <response code="422">If ConcurrencyStamps don't match.</response>
  [HttpPatch()]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product updatedProduct)
  {
    var result = await productService.UpdateProduct(updatedProduct);

    return result.IsSuccess switch
    {
      true => Ok(result.Value),
      false => Problem(result.ErrorMessage, null, result.StatusCode),
    };
  }

  private IActionResult? ValidatePaginationParams(int pageSize, int pageNum = 1) =>
    pageNum < 1 || pageSize < 1 || pageSize > 200
      ? BadRequest("Page and PageSize must be greater than 0 and PageSize less " + "than 200")
      : (IActionResult?)null;

  /// <summary>
  /// Gets IEnumerable of Product for given Ids.
  /// </summary>
  /// <param name="productsIds"></param>
  /// <returns>List of products.</returns>
  /// <response code="200">List of products.</response>
  /// <response code="404">If product ids doesn't exist.</response>
  [HttpPost("batch")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetSelectiveProducts([FromBody] List<Guid> productsIds)
  {
    if (productsIds.Count == 0)
    {
      return ValidationProblem(null, null, 400, "No products id  provided.", null, null);
    }

    var result = await productService.GetBatchProducts(productsIds);

    return MapToObjectResult(result);
  }

  private ObjectResult MapToObjectResult<T>(ServiceResult<T> result)
  {
    return result.IsSuccess switch
    {
      true => Ok(result.Value),
      false => Problem(result.ErrorMessage, null, result.StatusCode),
    };
  }
}
