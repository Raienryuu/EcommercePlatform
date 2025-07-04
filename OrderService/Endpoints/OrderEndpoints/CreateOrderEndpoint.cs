using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Endpoints.Requests;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CreateOrderEndpoint
{
  public static WebApplication MapCreateOrderEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Orders.CREATE_ORDER,
        async Task<Results<CreatedAtRoute<Order>, ProblemHttpResult, ValidationProblem>> (
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
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
          }
          if (userId.Equals(Guid.Empty))
          {
            return TypedResults.Problem("User Id is invalid", statusCode: 400);
          }

          var result = await orderService.CreateOrder(userId, orderRequest, ct);

          return result.IsSuccess
            ? TypedResults.CreatedAtRoute(
              result.Value,
              nameof(GetOrderEndpoint),
              new { orderId = result.Value?.OrderId }
            )
            : TypedResults.Problem(result.ErrorMessage, statusCode: result.StatusCode);
        }
      )
      .WithName(nameof(CreateOrderEndpoint));
    return app;
  }
}
