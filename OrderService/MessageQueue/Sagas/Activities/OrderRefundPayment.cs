using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Services;

namespace OrderService.MessageQueue.Sagas.Activities;

public class OrderRefundPaymentAcitivity(IStripePaymentService stripeService)
  : IStateMachineActivity<CancelOrderState, IOrderCancelledRemovedProductsReservation>
{
  public void Accept(StateMachineVisitor visitor)
  {
    visitor.Visit(this);
  }

  public async Task Execute(
    BehaviorContext<CancelOrderState, IOrderCancelledRemovedProductsReservation> context,
    IBehavior<CancelOrderState, IOrderCancelledRemovedProductsReservation> next
  )
  {
    await Handle(context);
    await next.Execute(context).ConfigureAwait(false);
  }

  private async Task Handle(
    BehaviorContext<CancelOrderState, IOrderCancelledRemovedProductsReservation> context
  )
  {
    await stripeService.RefundPaymentForOrder(context.Saga.CorrelationId, context.CancellationToken);
  }

  public Task Faulted<TException>(
    BehaviorExceptionContext<CancelOrderState, IOrderCancelledRemovedProductsReservation, TException> context,
    IBehavior<CancelOrderState, IOrderCancelledRemovedProductsReservation> next
  )
    where TException : Exception
  {
    return next.Faulted(context);
  }

  public void Probe(ProbeContext context)
  {
    context.CreateMessageScope(nameof(IOrderCancelledRemovedProductsReservation));
  }
}
