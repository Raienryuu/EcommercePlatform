public class Startup {
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration){
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services){

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
        
    }
}