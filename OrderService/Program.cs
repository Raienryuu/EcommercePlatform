using Common;
using FluentValidation;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

    builder.Services.AddProblemDetails();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var connectionString = BuildConnectionString(builder);
    builder.Services.Configure<StripeConfig>(builder.Configuration.GetRequiredSection(StripeConfig.KEY));
    builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

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
    builder.Services.AddScoped<IOrderService, Services.OrderService>();

    builder.Services.AddFluentValidationAutoValidation();

    var otel = builder.Services.AddOpenTelemetry();

    otel.ConfigureResource(resource =>
      resource.AddService(
        serviceName: builder.Environment.ApplicationName,
        serviceInstanceId: Environment.MachineName
      )
    );

    builder.Services.AddLogging(configure =>
      configure.AddOpenTelemetry(exporter => exporter.AddOtlpExporter())
    );

    otel.WithMetrics(metrics =>
    {
      metrics
        .AddAspNetCoreInstrumentation()
        // Metrics provides by ASP.NET Core in .NET 8
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        // Metrics provided by System.Net libraries
        .AddMeter("System.Net.Http")
        .AddMeter("System.Net.NameResolution")
        .AddMeter(InstrumentationOptions.MeterName)
        .AddOtlpExporter();
    });

    otel.WithTracing(tracing =>
    {
      tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddOtlpExporter();
    });

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
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler();
    }
    app.UseCors("DevPolicy");
    /*app.UseAuthorization();*/

    app.MapOrderEndpoints();
    app.MapPaymentEndpoints();

    app.Run();
  }

  static void CreateDevelopmentDatabase(OrderDbContext? dbContext)
  {
    var retry = 0;
    while (true)
    {
      retry++;
      try
      {
        dbContext?.Database.EnsureCreated();
      }
      catch
      {
        Thread.Sleep(4000);
        if (retry > 5)
        {
          throw;
        }
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
