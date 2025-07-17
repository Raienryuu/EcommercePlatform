using CartService.Options;
using CartService.Services;
using Common;
using FastEndpoints;
using FluentValidation;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CartService;

public class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    _ = builder.Services.AddAuthorization();
    _ = builder.Services.AddEndpointsApiExplorer();
    _ = builder.Services.AddSwaggerGen();

    _ = builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("RedisConfig"));

    _ = builder.Services.AddSingleton<RedisConnectionFactory>();
    _ = builder.Services.AddScoped<ICartRepository, RedisCartRepository>();
    _ = builder.Services.AddCors(static o =>
      o.AddPolicy(
        "DevPolicy",
        static policyBuilder =>
        {
          _ = policyBuilder
            .WithOrigins("http://localhost:4200")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader();
        }
      )
    );

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
      tracing.AddAspNetCoreInstrumentation();
      tracing.AddHttpClientInstrumentation();
      tracing.AddOtlpExporter();
    });

    _ = builder.Services.AddFastEndpoints();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      _ = app.UseSwagger();
      _ = app.UseSwaggerUI();
    }

    if (!app.Environment.IsDevelopment())
    {
      app.UseDefaultExceptionHandler();
    }

    _ = app.UseCors("DevPolicy");
    _ = app.UseFastEndpoints();
    // app.UseHttpsRedirection();

    app.Run();
  }
}
