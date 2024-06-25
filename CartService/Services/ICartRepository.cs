using CartService.Requests;

namespace CartService.Services
{
  public interface ICartRepository
  {
	public Task<Guid> CreateNewCart(Cart c);
	public Task<Guid> AddNewItem(UpdateCart c);
  }
}