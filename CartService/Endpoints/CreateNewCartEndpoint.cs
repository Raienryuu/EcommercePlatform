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
    var result = await cartProvider.CreateNewCart(cart);
    if (result.IsSuccess)
    {
      await SendCreatedAtAsync<GetCartEndpoint>(
        new { cartGuid = result.Value },
        result.Value,
        cancellation: ct
      );
    }
    else
    {
      await SendResultAsync(TypedResults.Problem(result.ErrorMessage, statusCode: result.StatusCode));
    }
  }
}
