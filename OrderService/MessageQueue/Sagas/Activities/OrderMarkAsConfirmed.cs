using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using OrderService.Logging;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;

namespace OrderService.MessageQueue.Sagas.Activities
{
  public class OrderMarkAsConfirmedActivity(OrderDbContext db, ILogger<OrderMarkAsConfirmedActivity> logger)
    : IStateMachineActivity<OrderState, IOrderReserved>
  {
    public void Accept(StateMachineVisitor visitor)
    {
      visitor.Visit(this);
    }

    public void Probe(ProbeContext context)
    {
      context.CreateMessageScope(nameof(IOrderCancellationRequest));
    }

    private async Task Handle(BehaviorContext<OrderState, IOrderReserved> context)
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

      order.Status = OrderStatus.Confirmed;
      db.Orders.Entry(order).State = EntityState.Modified;

      try
      {
        await db.SaveChangesAsync(context.CancellationToken);
      }
      catch (DbUpdateException)
      {
        logger.OrderChangesSaveFailure(order.OrderId);
      }
    }

    public async Task Execute(
      BehaviorContext<OrderState, IOrderReserved> context,
      IBehavior<OrderState, IOrderReserved> next
    )
    {
      await Handle(context);
      await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
      BehaviorExceptionContext<OrderState, IOrderReserved, TException> context,
      IBehavior<OrderState, IOrderReserved> next
    )
      where TException : Exception
    {
      return next.Faulted(context);
    }
  }
}
