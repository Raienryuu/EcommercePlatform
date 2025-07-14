namespace ProductService.Tests;

  public class TempFixture : IClassFixture<AppFixture>
  {
	protected readonly AppFixture _app;
	protected readonly HttpClient _client;

	public TempFixture(AppFixture appFixture)
	{
	  _app = appFixture;
	  _client = _app.CreateClient();
	}
  }

public class TempFixtureSecondary : IClassFixture<AppFixture>
{
  protected readonly AppFixture _app;
  protected readonly HttpClient _client;

  public TempFixtureSecondary(AppFixture appFixture)
  {
	_app = appFixture;
	_client = _app.CreateClient();
  }
}

