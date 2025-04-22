using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
public record CustomerInformation
{
  public required string FullName { get; set; }
  public required string Email { get; set; }
  public required string PhoneNumber { get; set; }
  public required string Address { get; set; }
  public required string City { get; set; }
  public required string ZIPCode { get; set; }
  public required string Country { get; set; }
}
