using IdentityService.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Tests.Unit.Fakes;

public class ApplicationDbContextFake : ApplicationDbContext
{
    public ApplicationDbContextFake() : base(new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase($"Users-{Guid.NewGuid()}")
      .Options) {}
}