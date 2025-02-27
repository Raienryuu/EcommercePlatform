using CartService.Requests;

namespace CartService.Services;

public interface ICartRepository
{
  public Task<Cart?> GetCart(Guid g);
  public Task<Guid> CreateNewCart(Cart c);
  public Task<Guid> UpdateCart(UpdateCart c);
  public Task DeleteCart(Guid g);
  public Task<Guid> UpdateWholeCart(UpdateCart c);
}
