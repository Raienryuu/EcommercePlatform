using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Models;
using ProductService.Tests.Fakes;

public class ProductDbContextFakeBuilder
{
	public ProductDbContext _db;

	public ProductDbContextFakeBuilder WithProducts()
	{
		var categoryId = _db.ProductCategories.First().Id;
		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "Red Cup",
			Description = "Fairly big cup",
			Price = 20,
			Quantity = 10,
		});
		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "White Cup",
			Description = "Fairly big cup",
			Price = 15,
			Quantity = 0,
		});
		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "Green Cup",
			Description = "Fairly big cup",
			Price = 30,
			Quantity = 50,
		});
		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "Blue Cup",
			Description = "Fairly big cup",
			Price = 25,
			Quantity = 6,
		});

		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "Yellow Cup",
			Description = "Fairly big cup",
			Price = 23,
			Quantity = 50,
		});
		_db.Products.Add(new Product
		{
			CategoryId = categoryId,
			Name = "T-shirt",
			Description = "One size fits all",
			Price = 1500,
			Quantity = 34,
		});
		_db.SaveChanges();
		return this;
	}

	public ProductDbContextFakeBuilder WithCategories()
	{
	  System.Diagnostics.Debug.WriteLine("adding categories dude");
		var parent = new ProductCategory
		{
			CategoryName = "Sample category"
		};
		_db.ProductCategories.Add(parent);
		_db.ProductCategories.Add(new ProductCategory{
			// Id = 2,
			CategoryName = "Category to update"
		});
		_db.ProductCategories.Add(new ProductCategory{
			// Id = 3,
			CategoryName = "Category to delete"
		});

	_db.ProductCategories.Add(new ProductCategory
		{
			// Id = 4,
			CategoryName = "Child Category",
			ParentCategory = parent
	});
	_db.SaveChanges();

	return this;
	}

	public ProductDbContext Build()
	{
		_db.SaveChanges();
		return _db;
	}
	//public ProductDbContext BuildFromEmptyWithData(string connectionString)
	//{
	//	var options = new DbContextOptionsBuilder<ProductDbContext>()
	//		.UseSqlServer(connectionString).Options;
	//	_db = new(options);
	//	_db.Database.EnsureCreated();
	//	WithCategories();
	//	_db.SaveChanges();
	//	WithProducts();
	//	return Build();
	//}
	public void FillWithTestData(ProductDbContext db)
	{
		_db = db;
		WithCategories();
		WithProducts();
	}

}