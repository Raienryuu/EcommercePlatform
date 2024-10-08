using Ocelot.DependencyInjection;
using Ocelot.Middleware;

new WebHostBuilder()
  .UseKestrel()
  .UseContentRoot(Directory.GetCurrentDirectory())
  .ConfigureAppConfiguration((hostingContext, config) =>
  {
    IWebHostEnvironment env = hostingContext.HostingEnvironment;
    config
      .AddEnvironmentVariables()
      .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
      .AddJsonFile("appsettings.json", true, true)
      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
      .AddJsonFile("ocelot.json")
      .AddJsonFile($"ocelot.{env.EnvironmentName}.json", false, true);
  })
  .ConfigureServices((context, s) =>
  {
    new Startup(context.Configuration).ConfigureServices(s);
    s.AddOcelot();
  })
  .ConfigureLogging((hostingContext, logging) =>
  {
    logging.SetMinimumLevel(LogLevel.Trace);
    logging.AddConsole();
  })
  .Configure(app => { app.UseOcelot().Wait(); })
  .Build()
  .Run();