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
	  var cut = Factory.Create<CreateNewCartEndpoint>(new FakeCartRepository());

	  await cut.HandleAsync(cart, default);
	  var res = (Guid)cut.Response!;

	  Assert.IsType<Guid>(res);
	}

	[Fact]
	public async void CreateNewCart_EmptyCart_ErrorCode()
	{
	  var cart = new Cart
	  {
		Products = []
	  };

	  var (_, res) = await App.Client
		.POSTAsync<CreateNewCartEndpoint, Cart, ErrorResponse>(cart);

	  Assert.Contains("must not be empty", res.Errors.ElementAt(0).Value.ElementAt(0));
	}

	[Fact]
	public async void CreateNewCart_AmountIsZeroOrLess_ErrorCode()
	{
	  var cart = new Cart
	  {
		Products = [
		new() {
		  Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0"),
		  Amount = 0
		}]
	  };

	  var (_, res) = await App.Client
		.POSTAsync<CreateNewCartEndpoint, Cart, ErrorResponse>(cart);

	  Assert.Contains("greater than '0'", res.Errors.ElementAt(0).Value.ElementAt(0));
	}

	[Fact]
	public async void AddCartItem_ValidItem_CartId()
	{
	  var cart = new Cart
	  {
		Products = [
		new() {
		  Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0"),
		  Amount = 5
		}]
	  };
	  var (_, res) = await App.Client
		.POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);
	  var updateCart = new UpdateCart
	  {
		CartGuid = res,
		Cart = new Cart
		{
		  Products = [new()
		  {
			Id = Guid.Parse("11187665-97a9-4200-a354-f1c2cbcb63e0"),
			Amount = 2
		  } ]
		}
	  };

	  var (httpRes, _) = await App.Client
		.PATCHAsync<AddCartItemEndpoint, UpdateCart, Guid>(updateCart);

	  Assert.True(httpRes.IsSuccessStatusCode);
	}

  }
}