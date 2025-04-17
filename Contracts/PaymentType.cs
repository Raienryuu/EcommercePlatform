using System.Text.Json.Serialization;

namespace Contracts
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum PaymentType
  {
    Cash = 0,
    Online = 1,
  }
}
