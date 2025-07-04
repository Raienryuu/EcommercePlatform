using Common;
using OrderService.Endpoints.Requests;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
  Task<ServiceResult<Order>> CreateOrder(Guid userId, CreateOrderRequest orderRequest, CancellationToken ct);
  Task<ServiceResult<Order>> GetOrder(Guid orderId, Guid userId, CancellationToken ct);
  Task<ServiceResult> SetDeliveryMethod(
    Guid orderId,
    Guid userId,
    OrderDelivery deliveryMethod,
    CancellationToken ct
  );
  Task<ServiceResult> CancelOrder(Guid orderId, Guid userId, CancellationToken ct);
  Task<List<Order>> GetUserOrders(Guid userId, CancellationToken ct);
}
