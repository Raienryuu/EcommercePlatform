using OrderService.Endpoints.Requests;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
  Task<Order> CreateOrder(Guid userId, CreateOrderRequest orderRequest, CancellationToken ct);
  Task<(Order?, string error)> GetOrder(Guid orderId, Guid userId, CancellationToken ct);
  Task<(bool isSuccess, string error)> SetDeliveryMethod(
    Guid orderId,
    Guid userId,
    OrderDelivery deliveryMethod,
    CancellationToken ct
  );
  Task<(bool isSuccess, string error)> CancelOrder(Guid orderId, Guid userId, CancellationToken ct);
  Task<List<Order>> GetUserOrders(Guid userId, CancellationToken ct);
}
