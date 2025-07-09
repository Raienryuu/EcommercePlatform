using System.Net;
using CartService.Endpoints;
using CartService.Models;
using CartService.Requests;
using CartService.Tests.Fixtures;
using FastEndpoints;
using FastEndpoints.Testing;

namespace CartService.Tests;

public class CartServiceTests(CartApp app) : TestBase<CartApp>
{
  [Fact]
  public async Task CreateNewCart_ValidCart_NewCardId()
  {
    var createRequest = new CreateNewCartRequest
    {
      Products =
      [
        new Product { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 5 },
      ],
    };
    var (_, value) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createRequest
    );

    Assert.IsType<Guid>(value);
  }

  [Fact]
  public async Task CreateNewCart_EmptyCart_ErrorCode()
  {
    var createRequest = new CreateNewCartRequest { Products = [] };

    var (response, _) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createRequest
    );

    var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    Assert.False(response.IsSuccessStatusCode);
    Assert.Contains("must not be empty", content);
  }

  [Fact]
  public async Task CreateNewCart_AmountIsZeroOrLess_ErrorCode()
  {
    var createRequest = new CreateNewCartRequest
    {
      Products = [new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 0 }],
    };

    var (response, _) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createRequest
    );

    var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    Assert.Contains("greater than '0'", content);
  }

  [Fact]
  public async Task DeleteCart_ExistingCartId_OKResponse()
  {
    var createRequest = new CreateNewCartRequest
    {
      Products = [new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 5 }],
    };
    var (_, res) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createRequest
    );
    var deleteCartRequest = new DeleteCartRequest { Id = res };

    var httpResponse = await app.Client.DELETEAsync<DeleteCartEndpoint, DeleteCartRequest>(deleteCartRequest);

    Assert.NotNull(httpResponse);
    Assert.True(httpResponse.IsSuccessStatusCode);
  }

  [Fact]
  public async Task DeleteCart_NonExistentCartId_OKResponse()
  {
    var guid = Guid.NewGuid();
    var deleteCartRequest = new DeleteCartRequest { Id = guid };

    var httpRes = await app.Client.DELETEAsync<DeleteCartEndpoint, DeleteCartRequest>(deleteCartRequest);

    Assert.True(httpRes.IsSuccessStatusCode);
  }

  [Fact]
  public async Task GetCart_ExistingCardId_Cart()
  {
    var createNewCartRequest = new CreateNewCartRequest
    {
      Products = [new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 5 }],
    };
    var (_, newCartGuid) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createNewCartRequest
    );
    var getCartRequest = new GetCartRequest { Id = newCartGuid };
    var (_, cart) = await app.Client.GETAsync<GetCartEndpoint, GetCartRequest, Cart?>(getCartRequest);

    Assert.NotNull(cart);
    Assert.True(cart.Products.Count > 0);
  }

  [Fact]
  public async Task GetCart_NonExistentCardId_NullCart()
  {
    var id = Guid.NewGuid();
    var getCartRequest = new GetCartRequest { Id = id };
    var (response, cart) = await app.Client.GETAsync<GetCartEndpoint, GetCartRequest, Cart?>(getCartRequest);

    Assert.Null(cart);
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task CreateNewCart_CartWithDuplicateProducts_CartWithMergedProduct()
  {
    const int PRODUCTS_AFTER_MERGING = 1;
    var createNewCartRequest = new CreateNewCartRequest
    {
      Products =
      [
        new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 5 },
        new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 8 },
      ],
    };
    var (_, id) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createNewCartRequest
    );

    var getCartRequest = new GetCartRequest { Id = id };
    var (_, finalCart) = await app.Client.GETAsync<GetCartEndpoint, GetCartRequest, Cart?>(getCartRequest);

    Assert.Equal(PRODUCTS_AFTER_MERGING, finalCart?.Products.Count);
  }

  [Fact]
  public async Task UpdateWholeCart_ValidItem_CartWithMergedProducts()
  {
    const int PRODUCTS_COUNT_AFTER_UPDATE = 1;
    var expectedGuid = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0");
    var createNewCartRequest = new CreateNewCartRequest
    {
      Products = [new() { Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(), Amount = 5 }],
    };
    var (_, newId) = await app.Client.POSTAsync<CreateNewCartEndpoint, CreateNewCartRequest, Guid>(
      createNewCartRequest
    );
    var updateCartRequest = new UpdateCartRequest
    {
      Id = newId,
      Products = [new() { Id = expectedGuid.ToString(), Amount = PRODUCTS_COUNT_AFTER_UPDATE }],
    };

    var _ = await app.Client.PUTAsync<UpdateCartEndpoint, UpdateCartRequest, Guid>(updateCartRequest);

    var getCartRequest = new GetCartRequest { Id = newId };
    var (_, finalCart) = await app.Client.GETAsync<GetCartEndpoint, GetCartRequest, Cart?>(getCartRequest);
    Assert.Equal(PRODUCTS_COUNT_AFTER_UPDATE, finalCart?.Products.Count);
    Assert.Equal(expectedGuid.ToString(), finalCart?.Products.First().Id);
  }
}
