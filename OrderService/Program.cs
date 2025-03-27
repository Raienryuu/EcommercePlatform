using System.Configuration;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.Endpoints;
using OrderService.Endpoints.Requests;
using OrderService.Options;
using OrderService.Services;
using OrderService.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderService;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var connectionString = BuildConnectionString(builder);
    builder.Services.Configure<StripeConfig>(builder.Configuration.GetRequiredSection(StripeConfig.KEY));

    builder.Services.AddDbContext<OrderDbContext>(o =>
    {
      o.UseSqlServer(connectionString);
    });

    builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();

    _ = builder.Services.AddCors(o =>
      o.AddPolicy(
        "DevPolicy",
        policyBuilder =>
        {
          policyBuilder.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyHeader();
        }
      )
    );

    MessageQueueUtils.Configure(builder.Configuration, builder.Services);

    builder.Services.AddScoped<IValidator<CreateOrderRequest>, OrderValidator>();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddLogging(c => c.AddSimpleConsole());

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();

      using var scope = app.Services.CreateAsyncScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
      CreateDevelopmentDatabase(dbContext);
    }
    /*app.UseHttpsRedirection();*/

    app.UseCors("DevPolicy");
    /*app.UseAuthorization();*/

    app.MapOrderEndpoints();
    app.MapPaymentEndpoints();

    app.Run();
  }

  static void CreateDevelopmentDatabase(OrderDbContext? dbContext)
  {
    while (true)
    {
      try
      {
        dbContext?.Database.EnsureCreated();
      }
      catch
      {
        Thread.Sleep(5000);
        continue;
      }
      break;
    }
  }

  static string BuildConnectionString(WebApplicationBuilder builder)
  {
    return builder.Configuration.GetConnectionString("Host")
      + builder.Configuration.GetConnectionString("User")
      + builder.Configuration.GetConnectionString("Password")
      + builder.Configuration.GetConnectionString("DefaultConnection");
  }
}
