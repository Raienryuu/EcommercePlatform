using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ProductService
{
  public class Program
  {
	public static void Main(string[] args)
	{
	  var builder = WebApplication.CreateBuilder(args);


	  builder.Services.AddControllers();
	  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	  builder.Services.AddEndpointsApiExplorer();
	  builder.Services.AddSwaggerGen(options =>
	  {
		options.SwaggerDoc("v1", new OpenApiInfo
		{
		  Version = "v1",
		  Title = "Products",
		  Description = "API to manage products store."
		});

		var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
	  });

	  builder.Services.AddCors(o => o.AddPolicy("DevPolicy", policyBuilder =>
	  {
		policyBuilder.WithOrigins("http://localhost:4200")
		  .AllowCredentials()
		  .AllowAnyHeader();
	  }));

	  var connectionString = BuildConnectionString(builder);

	  builder.Services.AddDbContext<ProductDbContext>(options =>
		options.UseSqlServer(connectionString)
	  );

	  builder.Services.AddLogging();

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
		var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

		foreach (var x in Enumerable.Range(0, 3))
		{
		  Thread.Sleep(150);
		  CreateDevelopmentDatabase(dbContext);
		}

		static void CreateDevelopmentDatabase(ProductDbContext dbContext)
		{
		  try
		  {
			dbContext.Database.EnsureCreated();
		  }
		  catch
		  {// do nothing
		  }
		}
	  }

	  app.MapControllers();



	  app.Run();
	}

	private static string BuildConnectionString(WebApplicationBuilder builder)
	{
	  return builder.Configuration.GetConnectionString("Host") +
			 builder.Configuration.GetConnectionString("User") +
			 builder.Configuration.GetConnectionString("Password") +
			 builder.Configuration.GetConnectionString("DefaultConnection");
	}
  }
}