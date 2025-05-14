using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.Activities;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas
{
  public class CancelOrderSaga : MassTransitStateMachine<CancelOrderState>
  {
    public CancelOrderSaga()
    {
      InstanceState(x => x.CurrentState);

      Initially(
        When(OrderCancellationRequest)
          .Activity(x => x.OfType<OrderCancelledStartCancellationActivity>())
          .TransitionTo(Pending)
      );

      During(
        Pending,
        When(OrderCancelledRemovedProductsReservation)
          .Then(static context =>
          {
            context.Saga.OrderProductsReservationCancelled = true;
          })
          .Activity(x => x.OfType<OrderRefundPaymentAcitivity>())
          .TransitionTo(Confirmed)
      );

      During(Pending, When(OrderCancelledCancellationUnavailable).TransitionTo(Final));

      During(Confirmed, When(OrderCancelledPaymentRefunded).TransitionTo(Refunded));

      During(Confirmed, When(OrderCancelledPaymentIntentCancelled).TransitionTo(Refunded));

      Event(() => OrderCancellationRequest, x => x.CorrelateById(context => context.Message.OrderId));
      Event(
        () => OrderCancelledRemovedProductsReservation,
        x => x.CorrelateById(context => context.Message.OrderId)
      );
      Event(
        () => OrderCancelledCancellationUnavailable,
        x => x.CorrelateById(context => context.Message.OrderId)
      );

      Event(() => OrderCancelledPaymentRefunded, x => x.CorrelateById(context => context.Message.OrderId));
      Event(
        () => OrderCancelledPaymentIntentCancelled,
        x => x.CorrelateById(context => context.Message.OrderId)
      );
    }

    public Event<IOrderCancellationRequest>? OrderCancellationRequest { get; set; }
    public Event<IOrderCancelledRemovedProductsReservation>? OrderCancelledRemovedProductsReservation { get; set; }
    public Event<IOrderCancelledCancellationUnavailable>? OrderCancelledCancellationUnavailable { get; set; }
    public Event<IOrderCancelledPaymentRefunded>? OrderCancelledPaymentRefunded { get; set; }
    public Event<IOrderCancelledPaymentIntentCancelled>? OrderCancelledPaymentIntentCancelled { get; set; }

    public State? Pending { get; set; }
    public State? Confirmed { get; set; }
    public State? Refunded { get; set; }
  }
}
