namespace OrderService.Endpoints;

public static class EndpointRoutes
{
  private const string ORDERS = "api/v1/orders";
  public const string GET_ORDER = ORDERS + "/{orderId:Guid}";
  public const string PUT_ORDER = ORDERS + "/{orderId:Guid}";
  public const string DELETE_ORDER = ORDERS + "/{orderId:Guid}";
  public const string CREATE_ORDER = ORDERS;
  public const string GET_USER_ORDERS = ORDERS;
}
