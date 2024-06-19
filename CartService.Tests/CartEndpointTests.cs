using CartService.Endpoints;
using CartService.Requests;
using CartService.Tests.Fixtures;
using FastEndpoints;
using FastEndpoints.Testing;

namespace CartService.Tests
{
  public class CartServiceTests(CartApp App) : TestBase<CartApp>
  {
	[Fact]
	public async void CreateNewCart_ValidCart_NewCardId()
	{
	  var cart = new Cart
	  {
		Products = [
		new() {
		  Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0"),
		  Amount = 5
		}]
	  };

	  var cut = Factory.Create<CreateNewCartEndpoint>(new FakeCartProvider());

	  await cut.HandleAsync(cart, CancellationToken.None);
	  var res = cut.Response;

	  Assert.IsType<Guid>(res);
	}

	[Fact]
	public async void CreateNewCart_EmptyCart_Error()
	{
	  var cart = new Cart
	  {
		Products = []
	  };

	  var (rsp, res) = await App.Client.POSTAsync<CreateNewCartEndpoint, Cart, Exception>(cart);


	  Assert.False(rsp.IsSuccessStatusCode);
	}
  }
}