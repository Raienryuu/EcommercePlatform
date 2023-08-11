
using Microsoft.Extensions.Options;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(o => o.AddPolicy("DevPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowCredentials()
                    .AllowAnyHeader();
            }));

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserOnly", policy =>
                    policy.RequireRole("User"));
            }
            );

            builder.Services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme();
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}