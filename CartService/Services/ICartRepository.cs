using CartService.Models;
using Common;

namespace CartService.Services;

public interface ICartRepository
{
  Task<ServiceResult<Cart>> GetCart(Guid g);
  Task<ServiceResult<Guid>> CreateNewCart(Cart c);
  Task<ServiceResult<Guid>> UpdateCart(Guid id, Cart c);
  Task<ServiceResult> DeleteCart(Guid g);
}
