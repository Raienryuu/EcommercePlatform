using Common;
using MassTransit.Futures.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Services;
using ProductService.Utility;
using ProductService.Validation;

namespace ProductService.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController(ProductDbContext db, IProductService productService) : ControllerBase
{
  /// <summary>
  /// Gets product with a given <paramref name="id"/>.
  /// </summary>
  /// <param name="id">Identifier of a product to find.</param>
  /// <returns><ref name="result">product</ref> object</returns>
  /// <response code="200">Found <ref name="Product">product</ref></response>
  /// <response code="404">If product doesn't exist</response>
  [HttpGet]
  [Route("{id:guid}")]
  [ProducesResponseType<Product>(StatusCodes.Status200OK)]
  [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProduct(Guid id)
  {
    var result = await productService.GetProduct(id);

    return result.IsSuccess ? Ok(result.Value) : Problem(result.ErrorMessage, null, result.StatusCode);
  }

  [HttpGet]
  [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
  [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetProductsPage([FromQuery] PaginationParams filters)
  {
    var validationResult = await new PaginationParamsValidator().ValidateAsync(filters);

    if (!validationResult.IsValid)
    {
      return ValidationProblem(new ValidationProblemDetails() { Errors = validationResult.ToDictionary() });
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
    var validationResult = await new PaginationParamsValidator().ValidateAsync(filters);

    if (!validationResult.IsValid)
    {
      return ValidationProblem(new ValidationProblemDetails() { Errors = validationResult.ToDictionary() });
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
    var validationResult = await new PaginationParamsValidator().ValidateAsync(filters);

    if (!validationResult.IsValid)
    {
      return ValidationProblem(new ValidationProblemDetails() { Errors = validationResult.ToDictionary() });
    }

    var products = await new ProductsPagination(filters, db)
      .GetPreviousPageQuery(filters.PageSize, referenceProduct)
      .ToListAsync();

    return Ok(products);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult> AddNewProduct([FromBody] Product newProduct)
  {
    var createdProductResult = await productService.AddProduct(newProduct);

    return createdProductResult.IsSuccess
      ? CreatedAtAction(
        nameof(GetProduct),
        new { id = createdProductResult.Value?.Id },
        createdProductResult.Value
      )
      : Problem(createdProductResult.ErrorMessage, null, createdProductResult.StatusCode);
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
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product updatedProduct)
  {
    var result = await productService.UpdateProduct(updatedProduct);

    return result.IsSuccess ? Ok(result.Value) : Problem(result.ErrorMessage, null, result.StatusCode);
  }

  /// <summary>
  /// Gets IEnumerable of Product for given Ids.
  /// </summary>
  /// <param name="productsIds"></param>
  /// <returns>List of products.</returns>
  /// <response code="200">List of products.</response>
  /// <response code="404">If product ids doesn't exist.</response>
  [HttpPost("batch")]
  [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProductsBatch([FromBody] List<Guid> productsIds)
  {
    if (productsIds.Count == 0)
    {
      return ValidationProblem(null, null, 400, "No products id  provided.", null, null);
    }

    var result = await productService.GetBatchProducts(productsIds);

    return result.IsSuccess ? Ok(result.Value) : Problem(result.ErrorMessage, null, result.StatusCode);
  }
}
