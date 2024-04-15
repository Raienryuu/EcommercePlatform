using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.OpenApi.Models;

namespace ProductService
{
  public static class Program
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
		  Description = "API to manage products store in database."
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

	  var connectionString =
		builder.Configuration.GetConnectionString("DefaultConnection");

	  builder.Services.AddDbContext<ProductDbContext>(options =>
		options.UseSqlServer(connectionString)
		  .LogTo(message => Debug.WriteLine(message))
	  );

	  var app = builder.Build();

	  //
	  if (app.Environment.IsDevelopment())
	  {
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
		  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
		  options.RoutePrefix = string.Empty;
		});
		app.UseCors("DevPolicy");
	  }


	  app.MapControllers();

	  app.Run();
	}
  }
}