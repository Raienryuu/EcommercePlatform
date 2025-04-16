using FluentValidation;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using OrderService.Endpoints.Requests;
using OrderService.Models;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CreateOrderEndpoint
{
  public static WebApplication MapCreateOrderEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Orders.CREATE_ORDER,
        async (
          OrderDbContext context,
          IPublishEndpoint publisher,
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

          var newOrder = new Order
          {
            OrderId = NewId.NextGuid(),
            UserId = userId,
            IsConfirmed = false,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            Notes = orderRequest.Notes,
            Products = orderRequest.Products,
            StripePaymentId = null,
            CurrencyISO = orderRequest.CurrencyISO,
          };

          _ = await context.Orders.AddAsync(newOrder, ct);
          _ = await context.SaveChangesAsync(ct);

          /// move to where delivery is set
          await publisher.Publish<IOrderCreatedByUser>(
            new
            {
              newOrder.OrderId,
              newOrder.Products,
              newOrder.CurrencyISO,
            },
            ct
          );
          //

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
