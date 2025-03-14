using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.Activities;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas;

public class NewOrderSaga : MassTransitStateMachine<OrderState>
{
  public NewOrderSaga()
  {
    InstanceState(x => x.CurrentState, Pending, Confirmed, ReadyToShip);

    Event(
      () => OrderSubmitted,
      x =>
      {
        _ = x.CorrelateById(context => context.Message.OrderId);
        x.InsertOnInitial = true;
        _ = x.SetSagaFactory(context => new OrderState { CorrelationId = context.Message.OrderId });
      }
    );

    Event(() => OrderReserved, x => x.CorrelateById(context => context.Message.OrderId));

    Event(() => OrderProductsNotAvailable, x => x.CorrelateById(context => context.Message.OrderId));

    Initially(
      When(OrderSubmitted)
        .Then(x => x.Saga.Products = x.Message.Products)
        .Publish(context => new ReserveOrderProductsCommand
        {
          OrderId = context.Saga.CorrelationId,
          Products = context.Saga.Products,
        })
        .TransitionTo(Pending)
    );

    During(Pending, When(OrderReserved).TransitionTo(Confirmed));

    During(
      Pending,
      When(OrderProductsNotAvailable)
        .Activity(x => x.OfType<OrderProductsNotAvaiableActivity>())
        .TransitionTo(Cancelled)
    );
  }

  public Event<IOrderSubmitted>? OrderSubmitted { get; set; }
  public Event<IOrderReserved>? OrderReserved { get; set; }
  public Event<IOrderProductsNotAvailable>? OrderProductsNotAvailable { get; set; }

  public State? Pending { get; set; }
  public State? Confirmed { get; set; }
  public State? ReadyToShip { get; set; }
  public State? Shipped { get; set; }
  public State? Cancelled { get; set; }
}
