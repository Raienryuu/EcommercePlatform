
using CartService.Options;
using CartService.Services;
using FastEndpoints;
namespace CartService;
public class Program
{
  private static void Main(string[] args)
  {
	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddAuthorization();
	// Add services to the container.
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.Configure<RedisOptions>(
	  builder.Configuration.GetSection("RedisConfig"));


	builder.Services.AddSingleton<RedisConnectionFactory>();
	builder.Services.AddScoped<ICartRepository, RedisCartRepository>();

	builder.Services.AddFastEndpoints();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
	  app.UseSwagger();
	  app.UseSwaggerUI();
	}

	app.UseFastEndpoints();
	app.UseHttpsRedirection();


	app.Run();
  }
}