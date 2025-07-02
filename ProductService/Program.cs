using System.Reflection;
using Common;
using FluentValidation;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductService.Endpoints;
using ProductService.Models;
using ProductService.Services;
using ProductService.Validation;

namespace ProductService;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddScoped<IProductService, Services.ProductService>();

    builder.Services.AddProblemDetails();
    builder.Services.AddControllers();

    builder.Services.AddScoped<IValidator<PaginationParams>, PaginationParamsValidator>();

    builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
          Version = "v1",
          Title = "Products",
          Description = "API to manage products store.",
        }
      );

      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    builder.Services.AddCors(o =>
      o.AddPolicy(
        "DevPolicy",
        policyBuilder =>
        {
          policyBuilder.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyHeader();
        }
      )
    );

    var connectionString = BuildConnectionString(builder);

    builder.Services.AddDbContext<ProductDbContext>(options =>
    {
      options.UseSqlServer(connectionString);
      options.EnableSensitiveDataLogging();
      options.ConfigureWarnings(w => w.Throw());
      options.LogTo(Console.WriteLine, LogLevel.Information);
    });

    var otel = builder.Services.AddOpenTelemetry();

    otel.ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName));

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

    MessageQueueUtils.Configure(builder);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
      });
      app.UseCors("DevPolicy");

      using var scope = app.Services.CreateAsyncScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
      CreateDevelopmentDatabase(dbContext);
      app.UseExceptionHandler();
    }

    app.MapDeliveryEndpoints();
    app.MapControllers();

    app.Run();
  }

  static void CreateDevelopmentDatabase(ProductDbContext? dbContext)
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

  private static string BuildConnectionString(WebApplicationBuilder builder)
  {
    return builder.Configuration.GetConnectionString("Host")
      + builder.Configuration.GetConnectionString("User")
      + builder.Configuration.GetConnectionString("Password")
      + builder.Configuration.GetConnectionString("DefaultConnection");
  }
}
