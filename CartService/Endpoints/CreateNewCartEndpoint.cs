using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class CreateNewCartEndpoint(ICartRepository cartProvider) : Endpoint<Cart>
{
  public override void Configure()
  {
    Post("api/cart");
    AllowAnonymous();
  }

  public override async Task HandleAsync(Cart req, CancellationToken ct)
  {
    var newId = await cartProvider.CreateNewCart(req);
    await SendCreatedAtAsync("api/cart/{guid}", newId, newId, cancellation: CancellationToken.None);
  }
}
