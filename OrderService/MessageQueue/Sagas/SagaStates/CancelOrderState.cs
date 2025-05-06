using MassTransit;

namespace OrderService.MessageQueue.Sagas.SagaStates;

public class CancelOrderState : SagaStateMachineInstance, ISagaVersion
{
  public Guid CorrelationId { get; set; }
  public bool OrderProductsReservatrionCancelled { get; set; }
  public bool OrderMarkedAsCancelled { get; set; }
  public string CurrentState { get; set; } = null!;
  public int Version { get; set; }
}
