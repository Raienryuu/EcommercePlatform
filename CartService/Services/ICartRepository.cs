using CartService.Models;

namespace CartService.Services;

public interface ICartRepository
{
  Task<Cart?> GetCart(Guid g);
  Task<Guid> CreateNewCart(Cart c);
  Task<Guid> UpdateCart(Guid id, Cart c);
  Task DeleteCart(Guid g);
}
