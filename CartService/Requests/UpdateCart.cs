namespace CartService.Requests;

public class UpdateCart
{
  public Guid CartGuid { get; set; }
  public required Cart Cart { get; set; }
}
