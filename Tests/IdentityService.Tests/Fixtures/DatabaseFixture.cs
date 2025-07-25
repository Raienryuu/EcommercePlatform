using IdentityService.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace IdentityService.Tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

  public string GetConnectionString() => _dbContainer.GetConnectionString();

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
  }

  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();
    var dbContext = new ApplicationDbContextFake(
      new DbContextOptionsBuilder<Data.ApplicationDbContext>()
        .UseSqlServer(_dbContainer.GetConnectionString())
        .Options
    );
    _ = dbContext.Database.EnsureCreated();
    dbContext.FillData();
  }
}
