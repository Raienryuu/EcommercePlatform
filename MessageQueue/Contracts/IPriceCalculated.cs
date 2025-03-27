namespace MessageQueue.Contracts;

public interface IOrderPriceCalculated
{
  Guid OrderId { get; }
  string CurrencyISO { get; }
  int TotalPriceInSmallestCurrencyUnit { get; }
}
