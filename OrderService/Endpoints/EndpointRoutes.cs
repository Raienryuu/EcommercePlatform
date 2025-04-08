namespace OrderService.Endpoints;

public static class EndpointRoutes
{
  public static class Orders
  {
    private const string ORDERS = "api/v1/orders";
    public const string GET_ORDER = ORDERS + "/{orderId:Guid}";
    public const string PUT_ORDER = ORDERS + "/{orderId:Guid}";
    public const string DELETE_ORDER = ORDERS + "/{orderId:Guid}";
    public const string CREATE_ORDER = ORDERS;
    public const string GET_USER_ORDERS = ORDERS;
  }

  public static class Payments
  {
    private const string PAYMENTS = "api/v1/payments";
    public const string CREATE_NEW_PAYMENT_SESSION = PAYMENTS + "/{orderId:Guid}";
    public const string GET_PAYMENT_STATUS = PAYMENTS + "/{orderId:Guid}";
  }
}
