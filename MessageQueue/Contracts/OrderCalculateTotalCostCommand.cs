using MessageQueue.DTOs;

namespace MessageQueue.Contracts;

public class OrderCalculateTotalCostCommand
{
  public required Guid OrderId { get; init; }
  public required OrderProductDTO[] Products { get; set; }
  public required string CurrencyISO { get; init; }
  public required decimal EurToCurrencyMultiplier { get; init; }
  public required Guid DeliveryId { get; init; }
}
