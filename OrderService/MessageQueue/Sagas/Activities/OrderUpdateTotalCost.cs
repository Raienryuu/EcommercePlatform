using MassTransit;
using MessageQueue.Contracts;
using OrderService.Logging;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas.Activities;

public class OrderUpdateTotalCostActivity(
  OrderDbContext orderDb,
  ILogger<OrderUpdateTotalCostActivity> logger
) : IStateMachineActivity<OrderState, IOrderPriceCalculated>
{
  public void Probe(ProbeContext context)
  {
    _ = context.CreateMessageScope(nameof(IOrderPriceCalculated));
  }

  public void Accept(StateMachineVisitor visitor)
  {
    visitor.Visit(this);
  }

  public async Task Execute(
    BehaviorContext<OrderState, IOrderPriceCalculated> context,
    IBehavior<OrderState, IOrderPriceCalculated> next
  )
  {
    await SetPriceOnOrder(context);
    await next.Execute(context).ConfigureAwait(false);
  }

  public Task Faulted<TException>(
    BehaviorExceptionContext<OrderState, IOrderPriceCalculated, TException> context,
    IBehavior<OrderState, IOrderPriceCalculated> next
  )
    where TException : Exception
  {
    return next.Faulted(context);
  }

  private async Task SetPriceOnOrder(BehaviorContext<OrderState, IOrderPriceCalculated> context)
  {
    var order = await orderDb.Orders.FindAsync([context.Saga.CorrelationId], context.CancellationToken);
    if (order is null)
    {
      logger.OrderNotFound();
      return;
    }

    if (order.CurrencyISO != context.Message.CurrencyISO)
    {
      logger.OrderCurrencyMismatch(order.CurrencyISO, context.Message.CurrencyISO);
      return;
    }

    if (order.TotalPriceInSmallestCurrencyUnit is not null)
    {
      logger.OrderPriceIsReassigned(context.Saga.CorrelationId, (int)order.TotalPriceInSmallestCurrencyUnit);
    }

    order.TotalPriceInSmallestCurrencyUnit = context.Message.TotalPriceInSmallestCurrencyUnit;
    try
    {
      logger.LogInformation(
        "Attemting to save {Total} for orderId: {OrderId}",
        order.TotalPriceInSmallestCurrencyUnit,
        order.OrderId
      );
      _ = await orderDb.SaveChangesAsync(context.CancellationToken);
    }
    catch (Exception e)
    {
      logger.LogCritical("Caught error: {Message}", e.Message);
      throw;
    }
  }
}
