using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderService.Endpoints.Requests;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CreateOrderEndpoint
{
  public static WebApplication MapCreateOrderEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Orders.CREATE_ORDER,
        async (
          IOrderService orderService,
          IValidator<CreateOrderRequest> validator,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromBody] CreateOrderRequest orderRequest
        ) =>
        {
          var validationResult = await validator.ValidateAsync(orderRequest);
          if (!validationResult.IsValid)
          {
            return Results.ValidationProblem(validationResult.ToDictionary());
          }
          if (userId.Equals(Guid.Empty))
          {
            return Results.BadRequest("User Id is invalid");
          }

          var newOrder = await orderService.CreateOrder(userId, orderRequest, ct);

          return Results.CreatedAtRoute(
            nameof(GetOrderEndpoint),
            new { orderId = newOrder.OrderId },
            newOrder
          );
        }
      )
      .WithName(nameof(CreateOrderEndpoint));
    return app;
  }
}
