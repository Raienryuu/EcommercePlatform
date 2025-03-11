using CartService.Options;
using CartService.Services;
using FastEndpoints;

namespace CartService;

public class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    _ = builder.Services.AddAuthorization();
    _ = builder.Services.AddEndpointsApiExplorer();
    _ = builder.Services.AddSwaggerGen();

    _ = builder.Services.Configure<RedisOptions>(
      builder.Configuration.GetSection("RedisConfig"));

    _ = builder.Services.AddSingleton<RedisConnectionFactory>();
    _ = builder.Services.AddScoped<ICartRepository, RedisCartRepository>();
    _ = builder.Services.AddCors(static o => o.AddPolicy("DevPolicy", static policyBuilder =>
      {
        _ = policyBuilder.WithOrigins("http://localhost:4200")
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader();
      }));

    _ = builder.Services.AddFastEndpoints();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      _ = app.UseSwagger();
      _ = app.UseSwaggerUI();
    }

    _ = app.UseCors("DevPolicy");
    _ = app.UseFastEndpoints();
    // app.UseHttpsRedirection();

    app.Run();
  }
}
