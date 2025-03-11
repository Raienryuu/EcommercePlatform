using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway;

new WebHostBuilder()
  .UseKestrel()
  .UseContentRoot(Directory.GetCurrentDirectory())
  .ConfigureAppConfiguration(static (hostingContext, config) =>
  {
    var env = hostingContext.HostingEnvironment;
    _ = config
      .AddEnvironmentVariables()
      .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
      .AddJsonFile("appsettings.json", true, true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
      .AddJsonFile("ocelot.json")
      .AddJsonFile($"ocelot.{env.EnvironmentName}.json", false, true);
  })
  .ConfigureServices(static (context, s) =>
  {
    new Startup(context.Configuration).ConfigureServices(s);
    _ = s.AddOcelot();
  })
  .ConfigureLogging(static (hostingContext, logging) =>
  {
    _ = logging.SetMinimumLevel(LogLevel.Trace);
    _ = logging.AddConsole();
  })
  .Configure(static app => { app.UseOcelot().Wait(); })
  .Build()
  .Run();
