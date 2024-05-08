using Microsoft.EntityFrameworkCore;

namespace OrderService
{
  public class Program
  {
	public static void Main(string[] args)
	{
	  var builder = WebApplication.CreateBuilder(args);

	  // Add services to the container.

	  builder.Services.AddControllers();
	  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	  builder.Services.AddEndpointsApiExplorer();
	  builder.Services.AddSwaggerGen();
	  var connectionString = BuildConnectionString(builder);
	  builder.Services.AddDbContext<OrderDbContext>(o =>
	  {
		o.UseSqlServer(connectionString);
		//o.AddInterceptors();
	  });

	  MessageQueueUtils.Configure(builder.Configuration, builder.Services);

	  var app = builder.Build();

	  // Configure the HTTP request pipeline.
	  if (app.Environment.IsDevelopment())
	  {
		app.UseSwagger();
		app.UseSwaggerUI();
	  }

	  app.UseHttpsRedirection();

	  app.UseAuthorization();


	  app.MapControllers();

	  using (var scope = app.Services.CreateAsyncScope())
	  {
		var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
		dbContext.Database.EnsureCreated();
	  }

	  app.Run();
	}

	private static string BuildConnectionString(WebApplicationBuilder builder)
	{
	  return builder.Configuration.GetConnectionString("Host") +
			 builder.Configuration.GetConnectionString("User") +
			 builder.Configuration.GetConnectionString("Password") +
			 builder.Configuration.GetConnectionString("DefaultConnection");
	}
  }
}
