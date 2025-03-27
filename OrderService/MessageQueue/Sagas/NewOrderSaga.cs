using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.Activities;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas;

public class NewOrderSaga : MassTransitStateMachine<OrderState>
{
  public NewOrderSaga()
  {
    InstanceState(x => x.CurrentState);

    Initially(
      When(OrderCreatedByUser)
        .Then(x =>
        {
          x.Saga.Products = x.Message.Products;
          x.Saga.CorrelationId = x.Message.OrderId;
          x.Saga.CurrencyISO = x.Message.CurrencyISO;
        })
        .Publish(ctx => new OrderCalculateTotalCostCommand()
        {
          OrderId = ctx.Saga.CorrelationId,
          CurrencyISO = ctx.Saga.CurrencyISO,
          EurToCurrencyMultiplier = 1m,
          Products = ctx.Saga.Products,
        })
        .TransitionTo(InCheckout)
    );

    During(InCheckout, When(OrderPriceCalculated).Activity(x => x.OfType<OrderUpdateTotalCostActivity>()));

    During(
      InCheckout,
      When(OrderSubmitted)
        .Publish(o => new ReserveOrderProductsCommand()
        {
          Products = o.Saga.Products,
          OrderId = o.Saga.CorrelationId,
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

    Event(() => OrderReserved, x => x.CorrelateById(context => context.Message.OrderId));
    Event(
      () => OrderCreatedByUser,
      x =>
      {
        x.CorrelateById(context => context.Message.OrderId);
        x.InsertOnInitial = true;
      }
    );
    Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
    Event(() => OrderProductsNotAvailable, x => x.CorrelateById(context => context.Message.OrderId));
    Event(() => OrderPriceCalculated, x => x.CorrelateById(context => context.Message.OrderId));
  }

  public Event<IOrderCreatedByUser>? OrderCreatedByUser { get; set; }
  public Event<IOrderSubmitted>? OrderSubmitted { get; set; }
  public Event<IOrderReserved>? OrderReserved { get; set; }
  public Event<IOrderProductsNotAvailable>? OrderProductsNotAvailable { get; set; }
  public Event<IOrderPriceCalculated>? OrderPriceCalculated { get; set; }

  public State? InCheckout { get; set; }
  public State? Pending { get; set; }
  public State? Confirmed { get; set; }
  public State? ReadyToShip { get; set; }
  public State? Shipped { get; set; }
  public State? Cancelled { get; set; }
}
