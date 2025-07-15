using System.Data.Common;
using Contracts;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderService.Endpoints.Requests;
using OrderService.Models;

namespace OrderService.Tests.Unit;

public class OrderServiceTests : IDisposable
{
  private readonly DbConnection _connection;
  private readonly DbContextOptions<OrderDbContext> _dbContextOptions;
  private readonly Mock<ILogger<Services.OrderService>> _loggerMock = new();
  private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();

  public OrderServiceTests()
  {
    _connection = new SqliteConnection("DataSource=:memory:");
    _connection.Open();
    _dbContextOptions = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(_connection).Options;
    using var context = new OrderDbContext(_dbContextOptions);
    context.Database.EnsureCreated();
  }

  public void Dispose()
  {
    _connection.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateOrder_ValidRequest_ReturnsSuccessWithOrder()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var orderRequest = new CreateOrderRequest
    {
      Notes = "Test order",
      Products =
      [
        new()
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 2,
          Price = 100,
        },
      ],
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);

    // Act
    var result = await service.CreateOrder(userId, orderRequest);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(201, result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal(userId, result.Value.UserId);
    Assert.Equal(orderRequest.Notes, result.Value.Notes);
    Assert.Equal(orderRequest.CurrencyISO, result.Value.CurrencyISO);
    Assert.Single(result.Value.Products);
    _publishEndpointMock.Verify(
      p => p.Publish<IOrderCreatedByUser>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
      Times.Once
    );
  }

  [Fact]
  public async Task CreateOrder_EmptyProducts_CreatesOrderWithNoProducts()
  {
    var userId = Guid.NewGuid();
    var orderRequest = new CreateOrderRequest
    {
      Notes = "No products",
      Products = [],
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CreateOrder(userId, orderRequest);
    Assert.True(result.IsSuccess);
    Assert.Empty(result.Value.Products);
  }

  [Fact]
  public async Task CreateOrder_ProductWithZeroQuantity_CreatesOrderWithZeroQuantity()
  {
    var userId = Guid.NewGuid();
    var orderRequest = new CreateOrderRequest
    {
      Notes = "Zero quantity",
      Products =
      [
        new()
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 0,
          Price = 100,
        },
      ],
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CreateOrder(userId, orderRequest);
    Assert.True(result.IsSuccess);
    Assert.Single(result.Value.Products);
    Assert.Equal(0, result.Value.Products.First().Quantity);
  }

  [Fact]
  public async Task CreateOrder_DuplicateProductIds_AllAccepted()
  {
    var userId = Guid.NewGuid();
    var productId = Guid.NewGuid();
    var orderRequest = new CreateOrderRequest
    {
      Notes = "Duplicate products",
      Products =
      [
        new()
        {
          Id = 1,
          ProductId = productId,
          Quantity = 1,
          Price = 100,
        },
        new()
        {
          Id = 2,
          ProductId = productId,
          Quantity = 2,
          Price = 200,
        },
      ],
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CreateOrder(userId, orderRequest);
    Assert.True(result.IsSuccess);
    Assert.Equal(2, result.Value.Products.Count);
    Assert.All(result.Value.Products, p => Assert.Equal(productId, p.ProductId));
  }

  [Fact]
  public async Task CreateOrder_LargeOrder_ReturnsSuccess()
  {
    var userId = Guid.NewGuid();
    var products = Enumerable
      .Range(1, 1000)
      .Select(i => new OrderProduct
      {
        Id = i,
        ProductId = Guid.NewGuid(),
        Quantity = 1,
        Price = 100,
      })
      .ToList();
    var orderRequest = new CreateOrderRequest
    {
      Notes = "Large order",
      Products = products,
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CreateOrder(userId, orderRequest);
    Assert.True(result.IsSuccess);
    Assert.Equal(1000, result.Value.Products.Count);
  }

  [Fact]
  public async Task CreateOrder_WithoutNotes_ReturnsSuccess()
  {
    var userId = Guid.NewGuid();
    var orderRequest = new CreateOrderRequest
    {
      Products =
      [
        new()
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        },
      ],
      CurrencyISO = "USD",
    };
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CreateOrder(userId, orderRequest);
    Assert.True(result.IsSuccess);
    Assert.Null(result.Value.Notes);
  }

  [Fact]
  public async Task SetDeliveryMethod_OrderExistsAndValid_SetsDeliveryAndReturnsSuccess()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    var delivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Express",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "John Doe",
        Email = "john@example.com",
        PhoneNumber = "1234567890",
        Address = "123 Main St",
        City = "Metropolis",
        ZIPCode = "12345",
        Country = "Freedonia",
      },
      Price = 10m,
    };
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);

    // Act
    var result = await service.SetDeliveryMethod(order.OrderId, userId, delivery, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.NotNull(updatedOrder!.Delivery);
    Assert.Equal(delivery.Name, updatedOrder.Delivery!.Name);
    _publishEndpointMock.Verify(
      p => p.Publish<OrderCalculateTotalCostCommand>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
      Times.Once
    );
  }

  [Fact]
  public async Task SetDeliveryMethod_OrderDoesNotExist_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var delivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Express",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "John Doe",
        Email = "john@example.com",
        PhoneNumber = "1234567890",
        Address = "123 Main St",
        City = "Metropolis",
        ZIPCode = "12345",
        Country = "Freedonia",
      },
      Price = 10m,
    };
    var result = await service.SetDeliveryMethod(Guid.NewGuid(), userId, delivery, CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task SetDeliveryMethod_UserDoesNotMatch_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var delivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Express",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "John Doe",
        Email = "john@example.com",
        PhoneNumber = "1234567890",
        Address = "123 Main St",
        City = "Metropolis",
        ZIPCode = "12345",
        Country = "Freedonia",
      },
      Price = 10m,
    };
    var result = await service.SetDeliveryMethod(
      order.OrderId,
      Guid.NewGuid(),
      delivery,
      CancellationToken.None
    );
    Assert.False(result.IsSuccess);
    Assert.Equal(401, result.StatusCode);
  }

  [Fact]
  public async Task SetDeliveryMethod_AlreadyHasDelivery_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    var delivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Express",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "John Doe",
        Email = "john@example.com",
        PhoneNumber = "1234567890",
        Address = "123 Main St",
        City = "Metropolis",
        ZIPCode = "12345",
        Country = "Freedonia",
      },
      Price = 10m,
    };
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
      Delivery = delivery,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var newDelivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Standard",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "Jane Doe",
        Email = "jane@example.com",
        PhoneNumber = "9876543210",
        Address = "456 Main St",
        City = "Metropolis",
        ZIPCode = "54321",
        Country = "Freedonia",
      },
      Price = 5m,
    };
    var result = await service.SetDeliveryMethod(order.OrderId, userId, newDelivery, CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(400, result.StatusCode);
  }

  [Fact]
  public async Task SetDeliveryMethod_OrderInInvalidStatus_AllowsSettingDelivery()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.Shipped,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var delivery = new OrderDelivery
    {
      DeliveryId = Guid.NewGuid(),
      HandlerName = "DHL",
      Name = "Express",
      DeliveryType = DeliveryType.DirectCustomerAddress,
      PaymentType = PaymentType.Online,
      CustomerInformation = new CustomerInformation
      {
        FullName = "John Doe",
        Email = "john@example.com",
        PhoneNumber = "1234567890",
        Address = "123 Main St",
        City = "Metropolis",
        ZIPCode = "12345",
        Country = "Freedonia",
      },
      Price = 10m,
    };
    var result = await service.SetDeliveryMethod(order.OrderId, userId, delivery, CancellationToken.None);
    Assert.True(result.IsSuccess);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.NotNull(updatedOrder!.Delivery);
    Assert.Equal(delivery.Name, updatedOrder.Delivery!.Name);
  }

  [Fact]
  public async Task CancelOrder_OrderExistsAndAwaitingConfirmation_CancelsOrderAndReturnsSuccess()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);

    // Act
    var result = await service.CancelOrder(order.OrderId, userId, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.Equal(OrderStatus.Cancelled, updatedOrder!.Status);
    _publishEndpointMock.Verify(
      p => p.Publish<IOrderCancellationRequest>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
      Times.Once
    );
  }

  [Fact]
  public async Task CancelOrder_OrderDoesNotExist_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.CancelOrder(Guid.NewGuid(), userId, CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task CancelOrder_UserDoesNotMatch_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.CancelOrder(order.OrderId, Guid.NewGuid(), CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(401, result.StatusCode);
  }

  [Fact]
  public async Task CancelOrder_AlreadyCancelled_ReturnsSuccessWithoutChangingStatus()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.Cancelled,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.CancelOrder(order.OrderId, userId, CancellationToken.None);
    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.Equal(OrderStatus.Cancelled, updatedOrder!.Status);
  }

  [Theory]
  [InlineData(OrderStatus.ReadyToShip)]
  [InlineData(OrderStatus.Shipped)]
  [InlineData(OrderStatus.Succeded)]
  public async Task CancelOrder_OrderInNonCancellableStatus_ReturnsFailure(OrderStatus status)
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = status,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.CancelOrder(order.OrderId, userId, CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(400, result.StatusCode);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.Equal(status, updatedOrder!.Status);
  }

  [Fact]
  public async Task CancelOrder_MultipleAttempts_IdempotentResult()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    // First cancel
    var result1 = await service.CancelOrder(order.OrderId, userId, CancellationToken.None);
    Assert.True(result1.IsSuccess);
    Assert.Equal(200, result1.StatusCode);
    // Second cancel (should be idempotent)
    var result2 = await service.CancelOrder(order.OrderId, userId, CancellationToken.None);
    Assert.True(result2.IsSuccess);
    Assert.Equal(200, result2.StatusCode);
    var updatedOrder = await context2.Orders.FindAsync(order.OrderId);
    Assert.Equal(OrderStatus.Cancelled, updatedOrder!.Status);
  }

  [Fact]
  public async Task GetOrder_OrderExistsAndUserMatches_ReturnsOrder()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);

    // Act
    var result = await service.GetOrder(order.OrderId, userId, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal(order.OrderId, result.Value.OrderId);
  }

  [Fact]
  public async Task GetOrder_OrderDoesNotExist_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.GetOrder(Guid.NewGuid(), userId, CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
  }

  [Fact]
  public async Task GetOrder_UserDoesNotMatch_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.GetOrder(order.OrderId, Guid.NewGuid(), CancellationToken.None);
    Assert.False(result.IsSuccess);
    Assert.Equal(401, result.StatusCode);
  }

  [Fact]
  public async Task GetOrder_CancelledOrder_ReturnsOrder()
  {
    var userId = Guid.NewGuid();
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.Cancelled,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.Add(order);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.GetOrder(order.OrderId, userId, CancellationToken.None);
    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    Assert.NotNull(result.Value);
    Assert.Equal(order.OrderId, result.Value.OrderId);
    Assert.Equal(OrderStatus.Cancelled, result.Value.Status);
  }

  [Fact]
  public async Task GetUserOrders_OrdersExistForUser_ReturnsOrdersList()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var order1 = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.AwaitingConfirmation,
    };
    var order2 = new Order
    {
      OrderId = Guid.NewGuid(),
      UserId = userId,
      CurrencyISO = "USD",
      Products = [
        new OrderProduct
        {
          Id = 1,
          ProductId = Guid.NewGuid(),
          Quantity = 1,
          Price = 100,
        }
      ],
      Status = OrderStatus.Confirmed,
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.AddRange(order1, order2);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);

    // Act
    var result = await service.GetUserOrders(userId, CancellationToken.None);

    // Assert
    Assert.Equal(2, result.Count);
    Assert.Contains(result, o => o.OrderId == order1.OrderId);
    Assert.Contains(result, o => o.OrderId == order2.OrderId);
  }

  [Fact]
  public async Task GetUserOrders_NoOrders_ReturnsEmptyList()
  {
    var userId = Guid.NewGuid();
    using var context = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context, _publishEndpointMock.Object);
    var result = await service.GetUserOrders(userId, CancellationToken.None);
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  [Fact]
  public async Task GetUserOrders_MixedStatuses_ReturnsAllOrders()
  {
    var userId = Guid.NewGuid();
    var orders = new[]
    {
      new Order
      {
        OrderId = Guid.NewGuid(),
        UserId = userId,
        CurrencyISO = "USD",
        Products = [
          new OrderProduct
          {
            Id = 1,
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            Price = 100,
          }
        ],
        Status = OrderStatus.AwaitingConfirmation,
      },
      new Order
      {
        OrderId = Guid.NewGuid(),
        UserId = userId,
        CurrencyISO = "USD",
        Products = [
          new OrderProduct
          {
            Id = 1,
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            Price = 100,
          }
        ],
        Status = OrderStatus.Confirmed,
      },
      new Order
      {
        OrderId = Guid.NewGuid(),
        UserId = userId,
        CurrencyISO = "USD",
        Products = [
          new OrderProduct
          {
            Id = 1,
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            Price = 100,
          }
        ],
        Status = OrderStatus.Cancelled,
      },
      new Order
      {
        OrderId = Guid.NewGuid(),
        UserId = userId,
        CurrencyISO = "USD",
        Products = [
          new OrderProduct
          {
            Id = 1,
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            Price = 100,
          }
        ],
        Status = OrderStatus.Shipped,
      },
    };
    using (var context = new OrderDbContext(_dbContextOptions))
    {
      context.Orders.AddRange(orders);
      context.SaveChanges();
    }
    using var context2 = new OrderDbContext(_dbContextOptions);
    var service = new Services.OrderService(_loggerMock.Object, context2, _publishEndpointMock.Object);
    var result = await service.GetUserOrders(userId, CancellationToken.None);
    Assert.Equal(4, result.Count);
    foreach (var order in orders)
      Assert.Contains(result, o => o.OrderId == order.OrderId && o.Status == order.Status);
  }
}
