using CartService.Mappers;
using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class CreateNewCartEndpoint(ICartRepository cartProvider) : Endpoint<CreateNewCartRequest, Guid>
{
  public override void Configure()
  {
    Post("api/cart");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateNewCartRequest req, CancellationToken ct)
  {
    var cart = req.ToCart();
    var newId = await cartProvider.CreateNewCart(cart);
    await SendCreatedAtAsync("api/cart/{guid}", newId, newId, cancellation: ct);
  }
}
