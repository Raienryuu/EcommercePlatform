namespace MessageQueue.Contracts;

public interface IOrderCancelledPaymentRefunded
{
  Guid OrderId { get; set; }
  int AmountInSmallestCurrencyUnit { get; set; }
  string CurrencyISO { get; set; }
}
