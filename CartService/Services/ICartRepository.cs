using CartService.Requests;

namespace CartService.Services
{
  public interface ICartRepository
  {
	public Guid CreateNewCart(Cart c);
  }
}