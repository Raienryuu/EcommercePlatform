using CartService.Requests;

namespace CartService.Services
{
  public interface ICartRepository
  {
	public Task<Guid> CreateNewCart(Cart c);
	public Task<Cart?> GetCart(Guid c);
	public Task DeleteCart(Guid c);
	public Task<Guid> AddNewItem(UpdateCart c);
  }
}