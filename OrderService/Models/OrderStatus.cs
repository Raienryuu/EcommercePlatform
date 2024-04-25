using MassTransit;

namespace Orders.Models
{
  public static class OrderStatus
  {

	public enum Type
	{
	  AwaitingConfirmation,
	  Confirmed,
	  ReadyToShip,
	  Shipped
	}

	public static Type FromString(string status)
	{
	  return status switch
	  {
		nameof(Type.AwaitingConfirmation) => Type.AwaitingConfirmation,
		nameof(Type.Confirmed) => Type.Confirmed,
		nameof(Type.ReadyToShip) => Type.ReadyToShip,
		nameof(Type.Shipped) => Type.Shipped,
		_ => throw new NotImplementedException("Invalid OrderStatus string given.")
	  };
	}
  }

}
