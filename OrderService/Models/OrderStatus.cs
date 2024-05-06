using MassTransit;

namespace OrderService.Models
{
  public static class OrderStatus
  {

	public enum Type
	{
	  AwaitingConfirmation,
	  Confirmed,
	  ReadyToShip,
	  Shipped,
	  Cancelled
	}

	public static Type FromString(string status)
	{
	  return status switch
	  {
		nameof(Type.AwaitingConfirmation) => Type.AwaitingConfirmation,
		nameof(Type.Confirmed) => Type.Confirmed,
		nameof(Type.ReadyToShip) => Type.ReadyToShip,
		nameof(Type.Shipped) => Type.Shipped,
		nameof(Type.Cancelled) => Type.Cancelled,
		_ => throw new NotImplementedException("Invalid OrderStatus string given.")
	  };
	}
  }

}
