using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;

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
      builder.Services.AddSwaggerGen();

      builder.Services.AddCors(o => o.AddPolicy("DevPolicy", policyBuilder =>
      {
        policyBuilder.WithOrigins("http://localhost:4200")
          .AllowCredentials()
          .AllowAnyHeader();
      }));

      builder.Configuration.Sources.Add(new JsonConfigurationSource
      {
        Path = "appsettings.Development.json",
        Optional = false,
        ReloadOnChange = true
      });

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