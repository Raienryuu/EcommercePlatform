using MassTransit;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.MessageQueue.Sagas
{
  public class CancelOrderSaga : MassTransitStateMachine<CancelOrderState>
  {
    public CancelOrderSaga() { }
  }
}
