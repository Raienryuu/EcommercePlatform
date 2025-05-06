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
      Initially(When(OrderCancellationRequest).Activity(x => x.OfType<OrderCancelledStartCancellationActivity>()).TransitionTo(Pending));
      During(Pending, When(OrderCancelledRemovedProductsReservation).TransitionTo(Confirmed));
      During(Pending, When(OrderCancelledCancellationUnavailable).TransitionTo(Final));

      During(Pending, When(OrderCancelledCancellationSuccessful).TransitionTo(Final));
    }


    public Event<IOrderCancellationRequest>? OrderCancellationRequest { get; set; }
    public Event<IOrderCancelledRemovedProductsReservation>? OrderCancelledRemovedProductsReservation { get; set; }
    public Event<IOrderCancelledCancellationUnavailable>? OrderCancelledCancellationUnavailable { get; set; }
    public Event<IOrderCancelledCancellationSuccessful>? OrderCancelledCancellationSuccessful { get; set; }


    public State? Pending { get; set; }
    public State? Confirmed { get; set; }
  }
}
