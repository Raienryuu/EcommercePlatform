﻿using MassTransit;
using MessageQueue.Contracts;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas.Activities
{
  public class OrderProductsNotAvaiableActivity(OrderDbContext db) : IStateMachineActivity<OrderState, IOrderProductsNotAvaiable>
  {
	private readonly OrderDbContext _db = db;

	public void Accept(StateMachineVisitor visitor)
	{
	  visitor.Visit(this);
	}

	public async Task Execute(BehaviorContext<OrderState, IOrderProductsNotAvaiable> context, IBehavior<OrderState, IOrderProductsNotAvaiable> next)
	{
	  SetOrderAsCancelled(context);
	  await next.Execute(context).ConfigureAwait(false);
	}

	public Task Faulted<TException>(BehaviorExceptionContext<OrderState, IOrderProductsNotAvaiable, TException> context, IBehavior<OrderState, IOrderProductsNotAvaiable> next) where TException : Exception
	{
	  return next.Faulted(context);
	}

	public void Probe(ProbeContext context)
	{
	  context.CreateMessageScope(nameof(IOrderProductsNotAvaiable));
	}

	private async void SetOrderAsCancelled(BehaviorContext<OrderState, IOrderProductsNotAvaiable> context)
	{
	  var order = await _db.Orders.FindAsync([context.Message.OrderId], cancellationToken: CancellationToken.None);
	  try
	  {
		order!.Status = Models.OrderStatus.Type.Cancelled;
		await _db.SaveChangesAsync(CancellationToken.None);
	  }
	  catch (Exception ex)
	  {
		throw new Exception("Unable to save changes to storage.", ex);
	  }
	}
  }
}
