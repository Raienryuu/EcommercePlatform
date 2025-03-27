namespace OrderService.Options;

public class StripeConfig
{
  public const string KEY = "Stripe";

  public required string ApiKey { get; set; }
}
