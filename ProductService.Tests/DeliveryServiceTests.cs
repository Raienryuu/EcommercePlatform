using Contracts;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Tests
{
  public class DeliveryServiceTests
  {
    [Fact]
    public async Task GetAvailableDeliveries_ExistingDeliveries_ReturnsListOfDeliveriesWithCorrectCountAndData()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

      using var context = new ProductDbContext(options);

      context.Deliveries.AddRange(
        new Delivery
        {
          DeliveryId = Guid.NewGuid(),
          HandlerName = "Handler 1",
          Name = "Delivery 1",
          DeliveryType = DeliveryType.DirectCustomerAddress,
          PaymentType = PaymentType.Online,
          Price = 10.00m,
        },
        new Delivery
        {
          DeliveryId = Guid.NewGuid(),
          HandlerName = "Handler 2",
          Name = "Delivery 2",
          DeliveryType = DeliveryType.DeliveryPoint,
          PaymentType = PaymentType.Online,
          Price = 15.00m,
        }
      );
      context.SaveChanges();
      var deliveryService = new DeliveryService(context);

      // Act
      var result = await deliveryService.GetAvailableDeliveries();

      // Assert
      Assert.True(result.IsSuccess);
      Assert.NotNull(result.Value);
      Assert.Equal(2, result.Value.Count);
      Assert.Equal("Delivery 1", result.Value[0].Name);
      Assert.Equal("Delivery 2", result.Value[1].Name);
      Assert.Equal("Handler 1", result.Value[0].HandlerName);
      Assert.Equal("Handler 2", result.Value[1].HandlerName);
      Assert.Equal(DeliveryType.DirectCustomerAddress, result.Value[0].DeliveryType);
      Assert.Equal(DeliveryType.DeliveryPoint, result.Value[1].DeliveryType);
      Assert.Equal(PaymentType.Online, result.Value[0].PaymentType);
      Assert.Equal(PaymentType.Online, result.Value[1].PaymentType);
      Assert.Equal(10.00m, result.Value[0].Price);
      Assert.Equal(15.00m, result.Value[1].Price);
    }

    [Fact]
    public async Task GetAvailableDeliveries_EmptyDatabase_ReturnsEmptyListInServiceResultValue()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;
      using var context = new ProductDbContext(options);
      var deliveryService = new DeliveryService(context);

      // Act
      var result = await deliveryService.GetAvailableDeliveries();

      // Assert
      Assert.True(result.IsSuccess);
      Assert.NotNull(result.Value);
      Assert.Empty(result.Value);
    }
  }
}
