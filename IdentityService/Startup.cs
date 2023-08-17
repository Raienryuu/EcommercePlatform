using IdentityService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace IdentityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
                .LogTo(message => Debug.WriteLine(message))
                );


            services.AddIdentity<IdentityUser, IdentityRole>(options => 
                options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddLogging(options =>
                options.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning)); 

            if (Configuration["ASPNETCORE_ENVIRONMENT"] == Environments.Development)
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
                services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowCredentials()
                    .AllowAnyHeader();
                }));
            }
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
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
                        .GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ecommerce platform API",
                    Version = "v1"
                });
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
                });
                app.UseExceptionHandler("/Error");
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy(
            new CookiePolicyOptions
                {
                    Secure = CookieSecurePolicy.Always
                }
            );

            app.UseCors("MyPolicy");

            // app.UseHttpsRedirection();

            // var roleManager = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()
            //     .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // var rolesToAdd = new List<string> { "User"};
            // foreach (var roleName in rolesToAdd)
            // {
            //     var roleExists = roleManager.RoleExistsAsync(roleName).Result;
            //     if (!roleExists)
            //     {
            //         roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            //     }
            // }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
