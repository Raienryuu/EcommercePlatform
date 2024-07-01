using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Tests.Fakes
{
  public static class FakeOrderDbContextBuilder
  {
	private static readonly DbContextOptionsBuilder<FakeOrderDbContext> optionsBuilder = new DbContextOptionsBuilder<FakeOrderDbContext>().UseInMemoryDatabase($"tempOrders-{Guid.NewGuid()}");


	public static void FillData(OrderDbContext ctx)
	{
	  ctx.Orders.Add(new Order
	  {
		OrderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7"),
		UserId = 1,
		Products = [
		  new OrderProduct{
			ProductId = 51,
			Quantity = 5,
		  }]
	  });
	  ctx.SaveChanges();
	}
  }
}
