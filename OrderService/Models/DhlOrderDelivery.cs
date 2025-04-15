namespace OrderService.Models;

public class DhlOrderDelivery : OrderDelivery
{
  DhlOrderDelivery()
  {
    HandlerName = "dhl";
  }
}
