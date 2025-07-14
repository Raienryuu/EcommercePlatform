using Testcontainers.MsSql;

namespace ProductService.Tests;

public class DatabaseFixture : IAsyncLifetime
{
  private readonly MsSqlContainer _msSqlContainer;

  public DatabaseFixture()
  {
    _msSqlContainer = new MsSqlBuilder()
      .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-GDR1-ubuntu-22.04")
      .Build();
  }

  public string GetConnectionString()
  {
    return _msSqlContainer.GetConnectionString();
  }

  public async Task DisposeAsync()
  {
    await _msSqlContainer.DisposeAsync();
  }

  public async Task InitializeAsync()
  {
    await _msSqlContainer.StartAsync();
  }
}
