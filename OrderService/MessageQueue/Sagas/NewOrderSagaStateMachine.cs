using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.SagaStates;
using System.Diagnostics;

namespace OrderService.MessageQueue.Sagas;
public class NewOrderStateMachine : MassTransitStateMachine<OrderState>
{
	public NewOrderStateMachine()
	{
	  InstanceState(x => x.CurrentState, Pending, Confirmed, ReadyToShip);

	  Event(() => OrderSubmitted, x =>
	  {
		x.CorrelateById(context => context.Message.OrderId);
		x.InsertOnInitial = true;
		x.SetSagaFactory(context => new OrderState
		{
		  CorrelationId = context.Message.OrderId,
		});
	  });

	  Event(() => OrderReserved, x => x.CorrelateById(context => context.Message.OrderId));
	  Event(() => OrderProductsNotAvaiable, x => x.CorrelateById(context => context.Message.OrderId));

	  Initially(
	   When(OrderSubmitted)
	   .Then(x =>
	   x.Saga.Products = x.Message.Products)
	   .Publish(context => new ReserveOrderProductsCommand
	   {
		 OrderId = context.Saga.CorrelationId,
		 Products = context.Saga.Products
	   })
	  .TransitionTo(Pending));

	  During(Pending,
	  When(OrderReserved)
	  .TransitionTo(Confirmed));

	  During(Pending,
	  When(OrderProductsNotAvaiable)
	  .TransitionTo(Cancelled));
	}

	public Event<IOrderSubmitted> OrderSubmitted { get; set; }
	public Event<IOrderReserved> OrderReserved { get; set; }
	public Event<IOrderProductsNotAvaiable> OrderProductsNotAvaiable { get; set; }

	public State Pending { get; set; }
	public State Confirmed { get; set; }
	public State Shipped { get; set; }
	public State ReadyToShip { get; set; }
	public State Cancelled { get; set; }
}
