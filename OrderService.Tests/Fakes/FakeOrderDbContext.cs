using Microsoft.EntityFrameworkCore;

namespace OrderService.Tests.Fakes
{
  public class FakeOrderDbContext(DbContextOptions options) : OrderDbContext(options)
  {
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
	  optionsBuilder.UseInMemoryDatabase($"tempOrders-{Guid.NewGuid()}");
	}

  }
}
