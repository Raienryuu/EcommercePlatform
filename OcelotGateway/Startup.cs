using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
  public IConfiguration Configuration { get; }
  public Startup(IConfiguration configuration)
  {
	Configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {

	var env = Configuration["ASPNETCORE_ENVIRONMENT"];
	if (env == Environments.Development)
	{
	  services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
	  {
		builder.WithOrigins("http://localhost:4200")
				  .AllowCredentials()
				  .AllowAnyHeader();
	  }));
	}

	var authKey = "Bearer";
	services.AddAuthentication(options =>
	{
	  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	}).AddJwtBearer(authKey, options =>
	{
	  options.TokenValidationParameters = new TokenValidationParameters
	  {
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = Configuration["Jwt:Issuer"],
		ValidAudience = Configuration["Jwt:Issuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
				  .GetBytes(Configuration["Jwt:Key"]!))
	  };
	});
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {

  }
}