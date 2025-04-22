using Contracts;
using OrderService.Models;

namespace OrderService.Tests.Fakes;

public static class FakeOrderDataInserter
{
  public static void FillData(OrderDbContext ctx)
  {
    var id = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");
    var isFilledAlready = ctx.Orders.Find(id);

    if (isFilledAlready is not null)
    {
      return;
    }

    _ = ctx.Orders.Add(
      new Order
      {
        OrderId = id,
        UserId = Guid.Parse("75699034-2ed6-4f39-a984-89bab648294c"),
        Products =
        [
          new OrderProduct
          {
            ProductId = Guid.NewGuid(),
            Quantity = 5,
            Price = 3213,
          },
        ],
        CurrencyISO = "eur",
        Delivery = new OrderDelivery()
        {
          HandlerName = "dhl",
          Price = 0,
          PaymentType = PaymentType.Online,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          Name = "Standard shipping",
        },
      }
    );
    _ = ctx.Orders.Add(
      new Order
      {
        OrderId = Guid.Parse("f1d0373d-6b65-419d-9308-55eabe156d1a"),
        UserId = Guid.Parse("75699034-2ed6-4f39-a984-89bab648294c"),
        Products =
        [
          new OrderProduct
          {
            ProductId = Guid.NewGuid(),
            Quantity = 8,
            Price = 431,
          },
        ],
        CurrencyISO = "eur",
        TotalPriceInSmallestCurrencyUnit = 101000,
        Delivery = new OrderDelivery()
        {
          HandlerName = "dhl",
          Price = 0,
          PaymentType = PaymentType.Online,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          Name = "Standard shipping",
        },
      }
    );
    _ = ctx.Orders.Add(
      new Order
      {
        OrderId = Guid.Parse("091ae438-aa1d-42cf-8058-2f89fcf313e2"),
        UserId = Guid.Parse("75699034-2ed6-4f39-a984-89bab648294c"),
        Products =
        [
          new OrderProduct
          {
            ProductId = Guid.NewGuid(),
            Quantity = 8,
            Price = 431,
          },
        ],
        CurrencyISO = "eur",
        TotalPriceInSmallestCurrencyUnit = 101000,
        StripePaymentId = "h12978yh897gh987h",
        Delivery = new OrderDelivery()
        {
          HandlerName = "dhl",
          Price = 0,
          PaymentType = PaymentType.Online,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          Name = "Standard shipping",
        },
      }
    );

    _ = ctx.SaveChanges();
  }
}
