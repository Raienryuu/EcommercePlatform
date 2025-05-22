namespace OrderService.Models;

public enum OrderStatus
{
  AwaitingConfirmation,
  Confirmed,
  ReadyToShip,
  Shipped,
  Cancelled,
  Succeded,
}
