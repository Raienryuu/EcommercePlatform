using CartService.Requests;

namespace CartService.Services
{
  public interface ICartRepository
  {
	public Task<Cart?> GetCart(Guid c);
	public Task<Guid> CreateNewCart(Cart c);
	public Task<Guid> UpdateCart(UpdateCart c);
	public Task DeleteCart(Guid c);
  }
}