using MassTransit;
using Microsoft.Extensions.Hosting;

namespace MessageQueue.Workers
{
  public class SampleWorker : BackgroundService
  {
	readonly IBus _bus;

	public SampleWorker(IBus bus)
	{
	  _bus = bus;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
	  while (!stoppingToken.IsCancellationRequested)
	  {

		await Task.Delay(5000, stoppingToken);
	  }
	}
  }
}
