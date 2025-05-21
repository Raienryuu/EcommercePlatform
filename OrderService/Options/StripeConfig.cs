namespace OrderService.Options;

public class StripeConfig
{
  public const string KEY = "Stripe";

  public required string ApiKey { get; set; }

  public required Webhooks Webhooks { get; set; }
}

public class Webhooks
{
  public required string PaymentIntentConfirmCode { get; set; }
  public required string RefundCode { get; set; }
}
