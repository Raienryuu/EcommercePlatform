using System.Text.Json.Serialization;

namespace Contracts
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum PaymentStatus
  {
    Pending = 0,
    Succeded = 1,
    Cancelled = 2,
    Failed = 3,
  }
}
