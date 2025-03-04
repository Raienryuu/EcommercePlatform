using MassTransit;
using ProductService.MessageQueue.Consumers;

namespace ProductService;

public static class MessageQueueUtils
{
  public static void Configure(WebApplicationBuilder builder)
  {
    var configuration = builder.Configuration;
    var services = builder.Services;
    try
    {
      var queueAddress = configuration.GetValue<string>("MQConfig:HostAddress");
      _ = ushort.TryParse(configuration.GetValue<string>("MQConfig:Port"), out ushort queuePort);
      var username = configuration.GetValue<string>("MQConfig:User");
      var userPassword = configuration.GetValue<string>("MQConfig:Pass");

      services.AddMassTransit(o =>
      {
        //var assembly = Assembly.GetEntryAssembly(); find consumers and others from Reflection
        o.AddConsumer<ReserveOrderProductsConsumer>();

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
    catch (NullReferenceException e)
    {
      throw new Exception("Invalid or empty message queue configuration data.", e);
    }
  }
}
