using MassTransit;
using MessageQueue.Contracts;
using OrderService.Logging;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;

namespace OrderService.MessageQueue.Sagas.Activities
{
  public class OrderCancelledStartCancellationActivity(
    OrderDbContext db,
    ILogger<OrderCancelledStartCancellationActivity> logger
  ) : IStateMachineActivity<CancelOrderState, IOrderCancellationRequest>
  {
    public void Accept(StateMachineVisitor visitor)
    {
      visitor.Visit(this);
    }

    public async Task Execute(
      BehaviorContext<CancelOrderState, IOrderCancellationRequest> context,
      IBehavior<CancelOrderState, IOrderCancellationRequest> next
    )
    {
      await Handle(context);
      await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
      BehaviorExceptionContext<CancelOrderState, IOrderCancellationRequest, TException> context,
      IBehavior<CancelOrderState, IOrderCancellationRequest> next
    )
      where TException : Exception
    {
      return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
      context.CreateMessageScope(nameof(IOrderCancellationRequest));
    }

    private async Task Handle(BehaviorContext<CancelOrderState, IOrderCancellationRequest> context)
    {
      var order = await db.Orders.FindAsync(
        [context.Message.OrderId],
        cancellationToken: CancellationToken.None
      );

      if (order is null)
      {
        logger.OrderNotFound();
        return;
      }

      order.IsCancelled = true;
      db.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
      await db.SaveChangesAsync(CancellationToken.None);

      await context.Publish(
        new OrderCancelledRemoveProductsReservationCommand() { OrderId = context.Message.OrderId }
      );
      logger.StartedOrderCancellation(order.OrderId);

      context.Saga.OrderMarkedAsCancelled = true;
    }
  }
}
