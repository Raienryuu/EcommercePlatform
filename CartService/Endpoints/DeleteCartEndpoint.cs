using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class DeleteCartEndpoint(ICartRepository cartProvider) : Endpoint<string>
{
  public override void Configure()
  {
    Delete("api/cart/{CartId}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(string req, CancellationToken ct)
  {
    var id = Guid.Parse(req);
    await cartProvider.DeleteCart(id);
    await SendAsync(null, cancellation: CancellationToken.None);
  }
}
