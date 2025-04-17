using System.Text.Json.Serialization;

namespace Contracts
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum DeliveryType
  {
    DeliveryPoint = 0,
    DirectCustomerAddress = 1,
  }
}
