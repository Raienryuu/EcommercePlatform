using MassTransit;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService;

public static class MessageQueueUtils
{
  public static void Configure(IConfiguration configuration, IServiceCollection services)
  {
    var queueAddress = configuration.GetValue<string>("MQConfig:HostAddress");
    _ = ushort.TryParse(configuration.GetValue<string>("MQConfig:Port"), out var queuePort);
    var username = configuration.GetValue<string>("MQConfig:User");
    var userPassword = configuration.GetValue<string>("MQConfig:Pass");

    _ = services.AddMassTransit(o =>
    {
      //var assembly = Assembly.GetEntryAssembly(); // find consumers and others from Reflection

      _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>()
        .MongoDbRepository(r =>
        {
          r.Connection = configuration["MongoDb:HostAddress"];
          r.DatabaseName = "ordersdb";
          r.CollectionName = "orders";
        })
        .Endpoint(e => e.Name = nameof(NewOrderSaga));

      o.UsingRabbitMq(
        (context, cfg) =>
        {
          cfg.Host(
            queueAddress,
            queuePort,
            "/",
            r =>
            {
              r.Username(username!);
              r.Password(userPassword!);
            }
          );

          cfg.ConfigureEndpoints(context);
        }
      );
    });
  }
}
