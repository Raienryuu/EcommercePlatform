using OrderService.Models;

namespace OrderService.Tests.Fakes;

public static class FakeOrderDataInserter
{
  public static void FillData(OrderDbContext ctx)
  {
    var id = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");
    _ = ctx.Orders.Add(
      new Order
      {
        OrderId = id,
        UserId = Guid.Parse("75699034-2ed6-4f39-a984-89bab648294c"),
        Products = [new OrderProduct { ProductId = Guid.NewGuid(), Quantity = 5 }],
      }
    );
    _ = ctx.SaveChanges();
  }
}
