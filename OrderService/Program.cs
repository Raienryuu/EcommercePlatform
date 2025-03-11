using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using OrderService.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderService;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var connectionString = BuildConnectionString(builder);
    builder.Services.AddDbContext<OrderDbContext>(o =>
    {
      o.UseSqlServer(connectionString);
    });

    MessageQueueUtils.Configure(builder.Configuration, builder.Services);

    builder.Services.AddScoped<IValidator<Order>, OrderValidator>();
    builder.Services.AddFluentValidationAutoValidation();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();

      using var scope = app.Services.CreateAsyncScope();
      using (var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>())
      {
        CreateDevelopmentDatabase(dbContext);
      }
      static void CreateDevelopmentDatabase(OrderDbContext? dbContext)
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
      app.UseHttpsRedirection();

      app.UseAuthorization();

      app.MapControllers();

      app.Run();
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
