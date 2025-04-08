namespace OrderService.Models;

public static class StripeHelpers
{
  public enum StripeOrderStatus
  {
    cancelled,
    processing,
    requires_action,
    requires_capture,
    requires_confirmation,
    requires_payment_method,
    succeeded,
  }

  public static string FromNumber(string status)
  {
    return status switch
    {
      "0" => StripeOrderStatus.cancelled.ToString(),
      "1" => StripeOrderStatus.processing.ToString(),
      "2" => StripeOrderStatus.requires_action.ToString(),
      "3" => StripeOrderStatus.requires_capture.ToString(),
      "4" => StripeOrderStatus.requires_confirmation.ToString(),
      "5" => StripeOrderStatus.requires_payment_method.ToString(),
      "6" => StripeOrderStatus.succeeded.ToString(),
      _ => throw new Exception("Not recognized payment status"),
    };
  }
}
