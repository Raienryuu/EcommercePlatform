using IdentityService.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace IdentityService.Tests;
public class DatabaseFixture : IAsyncLifetime
{
  public MsSqlContainer dbContainer = new MsSqlBuilder().Build();
  async Task IAsyncLifetime.DisposeAsync()
  {
	await dbContainer.DisposeAsync();

  }
  public async Task InitializeAsync()
  {
	await dbContainer.StartAsync();
	var dbContext = new ApplicationDbContextFake(new DbContextOptionsBuilder<Data.ApplicationDbContext>()
	  .UseSqlServer(dbContainer.GetConnectionString()).Options);
	dbContext.Database.EnsureCreated();
	dbContext.FillData();
  }
}
