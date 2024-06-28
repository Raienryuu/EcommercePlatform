using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Tests.Fakes
{
  public class FakeOrderDbContextBuilder : IDbContextFactory<OrderDbContext>
  {
	private static readonly DbContextOptionsBuilder<FakeOrderDbContext> optionsBuilder = new DbContextOptionsBuilder<FakeOrderDbContext>().UseInMemoryDatabase("tempOrders");

	private FakeOrderDbContext _db = new FakeOrderDbContext(optionsBuilder.Options);

	public OrderDbContext CreateDbContext()
	{
	  FillData();
	  _db.SaveChanges();
	  return (OrderDbContext)_db;
	}

	public FakeOrderDbContextBuilder FillData()
	{
	  _db.Orders.Add(new Order
	  {
		OrderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7"),
		UserId = 1,
		Products = [
		  new OrderProduct{
			ProductId = 51,
			Quantity = 5,
		  }]
	  });
	  return this;
	}
  }
}
