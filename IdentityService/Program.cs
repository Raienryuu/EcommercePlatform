using System.Text;
using Common;
using IdentityService.Data;
using IdentityService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
  builder.Configuration.GetConnectionString("DefaultConnection")
  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder
  .Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
    options.SignIn.RequireConfirmedAccount = false
  )
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IAddressesService, AddressesService>();

builder.Services.AddScoped<IUserService, UserService>();

if (
  builder.Configuration["ASPNETCORE_ENVIRONMENT"] == Environments.Development
  || builder.Configuration["ASPNETCORE_ENVIRONMENT"] == "docker"
)
{
  builder.Services.AddDatabaseDeveloperPageExceptionFilter();
  builder.Services.AddCors(o =>
    o.AddPolicy(
      "devel",
      builder =>
      {
        builder.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyHeader();
      }
    )
  );
}

builder
  .Services.AddAuthentication(options =>
  {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(
    "Bearer",
    options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
      };
    }
  );

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce platform API", Version = "v1" });
});

builder.Services.AddControllers();

var otel = builder.Services.AddOpenTelemetry();

otel.ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName));

builder.Services.AddLogging(configure => configure.AddOpenTelemetry(exporter => exporter.AddOtlpExporter()));

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

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration["ASPNETCORE_ENVIRONMENT"] == "docker")
{
  app.UseMigrationsEndPoint();
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
  });

  using var scope = app.Services.CreateAsyncScope();
  using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  CreateDevelopmentDatabase(dbContext);
}
else
{
  app.UseExceptionHandler();

  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });

app.UseCors("devel");

// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void CreateDevelopmentDatabase(ApplicationDbContext? dbContext)
{
  while (true)
  {
    try
    {
      dbContext?.Database.EnsureCreated();
    }
    catch
    {
      Thread.Sleep(2000);
      continue;
    }
    break;
  }
}


public partial class Program {}
