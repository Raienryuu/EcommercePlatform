using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace OcelotBasic
{
  public class Program
  {
	public static void Main(string[] args)
	{
	  new WebHostBuilder()
	  .UseKestrel()
	  .UseContentRoot(Directory.GetCurrentDirectory())
	  .ConfigureAppConfiguration((hostingContext, config) =>
	  {
		config
				  .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
				  .AddJsonFile("appsettings.json", true, true)
				  .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
				  .AddJsonFile("ocelot.json")
				  .AddJsonFile("ocelot.generic.json")
				  .AddEnvironmentVariables();
	  })
	  .ConfigureServices((context, s) =>
	  {
		new Startup(context.Configuration).ConfigureServices(s);
		s.AddOcelot();
	  })
	  .ConfigureLogging((hostingContext, logging) =>
	  {
	  })
	  .Configure(app =>
	  {
		app.UseOcelot().Wait();
	  })
	  .Build()
	  .Run();
	}
  }
}