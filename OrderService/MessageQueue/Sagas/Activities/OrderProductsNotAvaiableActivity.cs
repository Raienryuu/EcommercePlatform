using MassTransit;
using MessageQueue.Contracts;
using OrderService.Logging;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas.Activities
{
  public class OrderProductsNotAvaiableActivity(
    OrderDbContext db,
    ILogger<OrderProductsNotAvaiableActivity> logger
  ) : IStateMachineActivity<OrderState, IOrderProductsNotAvailable>
  {
    private readonly OrderDbContext _db = db;

    public void Accept(StateMachineVisitor visitor)
    {
      visitor.Visit(this);
    }

    public async Task Execute(
      BehaviorContext<OrderState, IOrderProductsNotAvailable> context,
      IBehavior<OrderState, IOrderProductsNotAvailable> next
    )
    {
      await SetOrderAsCancelled(context);
      await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
      BehaviorExceptionContext<OrderState, IOrderProductsNotAvailable, TException> context,
      IBehavior<OrderState, IOrderProductsNotAvailable> next
    )
      where TException : Exception
    {
      return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
      context.CreateMessageScope(nameof(IOrderProductsNotAvailable));
    }

    private async Task SetOrderAsCancelled(BehaviorContext<OrderState, IOrderProductsNotAvailable> context)
    {
      var order = await _db.Orders.FindAsync(
        [context.Message.OrderId],
        cancellationToken: CancellationToken.None
      );

      if (order is null)
      {
        logger.OrderNotFound();
        return;
      }

      order.Status = Models.OrderStatus.Cancelled;
      await _db.SaveChangesAsync(CancellationToken.None);
    }
  }
}
