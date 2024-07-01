using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Tests.Fakes
{
  public static class FakeOrderDataInserter
  {
	public static void FillData(OrderDbContext ctx)
	{
	  var id = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");
	  ctx.Orders.Add(new Order
	  {
		OrderId = id,
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
