﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService.Models;

namespace ProductService.Controllers.v1
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class ProductCategoryController(ProductDbContext context) : ControllerBase
  {
	private readonly ProductDbContext _context = context;

	// GET: api/ProductCategories
	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
	{
	  return await _context.ProductCategories.ToListAsync();
	}

	// GET: api/ProductCategories/5
	[HttpGet("{id}")]
	public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
	{
	  var productCategory = await _context.ProductCategories.FindAsync(id);

	  if (productCategory == null)
	  {
		return NotFound("No category found with a given ID.");
	  }

	  return (productCategory);
	}

	// GET: api/ProductCategories/releated/5
	[HttpGet("releated/{id}")]
	public async Task<ActionResult<IEnumerable<ProductCategory>>> GetReleatedCategories(int id)
	{
	  var productCategory = await _context.ProductCategories.FindAsync(id);

	  if (productCategory == null)
	  {
		return NotFound("No category found with a given ID.");
	  }

	  var releatedChildCategories = await _context.ProductCategories.Where(x => x.ParentCategory!.Id == productCategory.Id).ToListAsync();
	  if (productCategory.ParentCategory != null)
	  {
		var parentCategories = await _context.ProductCategories.Where(
			x => x.Id == productCategory.ParentCategory.Id).ToListAsync();
		releatedChildCategories.AddRange(parentCategories);
	  }

	  return Ok(releatedChildCategories);
	}

	// PUT: api/ProductCategories/5
	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPatch("{id}")]
	public async Task<ActionResult> PatchProductCategory(int id, ProductCategory productCategory)
	{
	  if (id != productCategory.Id)
	  {
		return BadRequest();
	  }

	  if (!ProductCategoryExists(id)) return NotFound("No category found with a given ID.");


	  if (!AssignParent(productCategory)) return BadRequest("Parent category not found.");
	  var localCategory = await _context.ProductCategories.FindAsync(productCategory.Id);

	  localCategory = productCategory;

	  await _context.SaveChangesAsync();

	  return NoContent();
	}

	// POST: api/ProductCategories
	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPost]
	public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
	{
	  var existingCategory = await _context.ProductCategories.Where(_ => _.CategoryName == productCategory.CategoryName).FirstOrDefaultAsync();

	  if (existingCategory != null)
	  {
		return Conflict("Category already exists.");
	  }

	  if (!AssignParent(productCategory)) return BadRequest("Parent category not found.");

	  _context.ProductCategories.Add(productCategory);

	  await _context.SaveChangesAsync();

	  return CreatedAtAction("GetProductCategory", new { id = productCategory.Id }, productCategory);
	}

	// DELETE: api/ProductCategories/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProductCategory(int id)
	{
	  var productCategory = await _context.ProductCategories.FindAsync(id);
	  if (productCategory == null)
	  {
		return NotFound("No category found with a given ID.");
	  }

	  _context.ProductCategories.Remove(productCategory);
	  await _context.SaveChangesAsync();

	  return NoContent();
	}

	private bool ProductCategoryExists(int id)
	{
	  return _context.ProductCategories.Any(e => e.Id == id);
	}

	private bool AssignParent(ProductCategory productCategory)
	{
	  if (productCategory.ParentCategory is null) return true;
	  var parent = _context.ProductCategories.FirstOrDefault(x => x.Id == productCategory.ParentCategory.Id);
	  if (parent?.CategoryName == productCategory.ParentCategory.CategoryName)
	  {
		productCategory.ParentCategory = parent;
		return true;
	  }
	  return false;
	}
  }
}
