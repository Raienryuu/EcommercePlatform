using MassTransit;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;


namespace OrderService;

public static class MessageQueueUtils
{
  public static void Configure(IConfiguration configuration, IServiceCollection services)
  {

	var queueAddress = configuration["MQConfig:HostAddress"];
	ushort.TryParse(configuration["MQConfig:Port"], out ushort queuePort);
	var username = configuration["MQConfig:User"];
	var userPassword = configuration["MQConfig:Pass"];


	services.AddMassTransit(o =>
	{
	  //var assembly = Assembly.GetEntryAssembly(); // find consumers and others from Reflection

	  o.AddSagaStateMachine<NewOrderSaga, OrderState>()
	  .MongoDbRepository(r =>
	  {
		r.Connection = configuration["MongoDb:HostAddress"];
		r.DatabaseName = "ordersdb";
		r.CollectionName = "orders";
	  })
	  .Endpoint(e => e.Name = nameof(NewOrderSaga));

	  o.UsingRabbitMq((context, cfg) =>
	   {
		 cfg.Host(queueAddress, queuePort, "/", r =>
		 {
		   r.Username(username);
		   r.Password(userPassword);
		 });

		 cfg.ConfigureEndpoints(context);
	   });

	});
  }
}
