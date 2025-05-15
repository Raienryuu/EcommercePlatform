using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class GetCartProductsCountEndpoint(ICartRepository cartProvider) : Endpoint<GetCartRequest, int>
{
  public override void Configure()
  {
    Get("api/cart/{@cartGuid}/count", static x => new { x.Id });
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetCartRequest request, CancellationToken ct)
  {
    var cart = await cartProvider.GetCart(request.Id);
    var distinctProductsCount = cart?.Products.Count ?? 0;
    await SendAsync(distinctProductsCount, cancellation: ct);
  }
}
