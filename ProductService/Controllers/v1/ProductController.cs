using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Controllers
{
  [ApiController]
  [Route("/api/v1/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductDbContext _db;

    public ProductsController(
        ILogger<ProductsController> logger,
        ProductDbContext db)
    {
      _logger = logger;
      _db = db;
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
      var result = await _db.Products.SingleOrDefaultAsync(p => p.Id == id);
      if (result is not null)
      {
        return Ok(result);
      }
      return NoContent();
    }

    [HttpGet]
    [Route("{page}/{pageSize}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsPage(
         int page, int pageSize, [FromQuery] SearchFilters? filters)
    {
      if (page < 1 || pageSize < 1 || pageSize > 200)
      {
        return BadRequest("Page and PageSize must be greater than 0 and less than 200");
      }

      IQueryable<Product> query = ApplySearchFilters(filters);

      var products = await query.Skip((page - 1) * pageSize)
          .Take(pageSize).ToListAsync();
      if(products.Count == 0)
      {
        return NoContent();
      }
      return Ok(products);
    }

    private IQueryable<Product> ApplySearchFilters(SearchFilters? filters)
    {
      var query = _db.Products.AsQueryable();

      if (filters is null)
      {
        return query;
      }

      if (filters.Name is not null)
      {
        query = query.Where(p => p.Name.Contains(
          filters.Name, StringComparison.OrdinalIgnoreCase));
      }
      if (filters.MinPrice is not null)
      {
        query = query.Where(p => p.Price >= filters.MinPrice);
      }
      if (filters.MaxPrice is not null)
      {
        query = query.Where(p => p.Price <= filters.MaxPrice);
      }
      if (filters.MinQuantity is not null)
      {
        query = query.Where(p => p.Quantity >= filters.MinQuantity);
      }

      // TODO: product sorting
      // query = query.OrderBy(p => p.Name);

      return query;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> AddNewProduct([FromBody] Product newProduct)
    {
      newProduct.Category = await _db.ProductCategories
          .SingleOrDefaultAsync(c => c.Id == newProduct.CategoryId);

      if (newProduct.Category is null)
      {
        return BadRequest(CreateErrorResponse("Category not found"));
      }

      _db.Products.Add(newProduct);
      await _db.SaveChangesAsync();

      var createdUri = Url.Action(
        "GetProduct",
        "Products",
        new { id = newProduct.Id });

      return base.Created(createdUri!, newProduct);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
      var oldProduct = await _db.Products.SingleOrDefaultAsync(p => p.Id == id);
      if (oldProduct is null)
      {
        return NotFound("Product not found");
      }

      if (updatedProduct.ConcurrencyStamp is not null &&
          oldProduct.ConcurrencyStamp!.SequenceEqual(updatedProduct.ConcurrencyStamp))
      {

        oldProduct.Price = updatedProduct.Price;
        oldProduct.Quantity = updatedProduct.Quantity;
        oldProduct.Name = updatedProduct.Name;
        oldProduct.Description = updatedProduct.Description;

        oldProduct.RefreshConcurrencyStamp();
        await _db.SaveChangesAsync();

        return Ok(oldProduct);
      }

      return UnprocessableEntity(CreateErrorResponse("ConcurrencyStamp mismatch"));
    }

    private static string CreateErrorResponse(string message)
    {
      return $"{{\"type\": \"error\", \"message\": \"{message}\"}}";
    }
  }
}