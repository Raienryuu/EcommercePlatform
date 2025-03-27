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
    var mongoSection = configuration.GetRequiredSection("MongoDb");

    //var assembly = Assembly.GetEntryAssembly(); // find consumers and others from Reflection
    _ = services.AddMassTransit(o =>
    {
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

      _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>()
        .MongoDbRepository(r =>
        {
          r.Connection = mongoSection.GetValue<string>("HostAddress");
          r.DatabaseName = "ordersdb";
          r.CollectionName = "orders";
        })
        .Endpoint(e => e.Name = nameof(NewOrderSaga));
    });
  }
}
