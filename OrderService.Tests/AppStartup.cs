namespace OrderService.Tests
{

  public class AppStartup : IClassFixture<AppFactory>
{
	protected readonly AppFactory _app;
	protected readonly HttpClient _client;
	
	public AppStartup(AppFactory appFactory)
	{
	  _app = appFactory;
	  _client = _app.CreateClient();
	}
  }
}
