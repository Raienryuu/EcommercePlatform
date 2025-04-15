using MassTransit;

namespace OrderService.MessageQueue.Sagas.SagaStates;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
  public Guid CorrelationId { get; set; }
  public int Version { get; set; }

  public string CurrentState { get; set; } = null!;
  /*public int CurrentState { get; set; }*/
}

public class CancelOrderState : OrderState
{
  public bool ProductsUnreserved { get; set; }
  public bool OrderMarkedAsCancelled { get; set; }
}
