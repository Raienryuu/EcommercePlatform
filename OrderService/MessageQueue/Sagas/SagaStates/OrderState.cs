using MassTransit;
using MessageQueue.DTOs;

namespace OrderService.MessageQueue.Sagas.SagaStates;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
  public Guid CorrelationId { get; set; }
  public int CurrentState { get; set; }
  public OrderProductDTO[] Products { get; set; }
  public int Version { get; set; }
}

public class CancelOrderState : OrderState
{
  public bool ProductsUnreserved { get; set; }
  public bool OrderMarkedAsCancelled { get; set; }
}
