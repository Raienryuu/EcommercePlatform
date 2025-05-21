using Microsoft.AspNetCore.Cors.Infrastructure;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway;

new WebHostBuilder()
  .UseKestrel()
  .UseContentRoot(Directory.GetCurrentDirectory())
  .ConfigureAppConfiguration(
    static (hostingContext, config) =>
    {
      var env = hostingContext.HostingEnvironment;
      _ = config
        .AddEnvironmentVariables()
        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
        .AddJsonFile("ocelot.json")
        .AddJsonFile($"ocelot.{env.EnvironmentName}.json", false, true);
    }
  )
  .ConfigureServices(
    static (context, s) =>
    {
      new Startup(context.Configuration).ConfigureServices(s);
      s.AddCors(c =>
      {
        c.AddDefaultPolicy(
          new CorsPolicyBuilder()
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .Build()
        );
      });

      _ = s.AddOcelot();
    }
  )
  .ConfigureLogging(
    static (hostingContext, logging) =>
    {
      _ = logging.AddConsole().SetMinimumLevel(LogLevel.None);
    }
  )
  .Configure(static app =>
  {
    app.UseCors();
    app.UseOcelot().Wait();
  })
  .Build()
  .Run();
