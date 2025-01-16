using ImageService;
using ImageService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.Configure<ConnectionOptions>(
            builder.Configuration.GetRequiredSection(ConnectionOptions.Key));
        var objectSerializer
            = new ObjectSerializer(ObjectSerializer.AllAllowedTypes);
        BsonSerializer.RegisterSerializer(objectSerializer);
        builder.Services.AddSingleton<IImageService, MongoImageService>();
        builder.Services.AddSingleton<MongoImageService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.MapGet("api/v1/name", async ([FromQuery] string productId,
                [FromServices] MongoImageService images,
                [FromQuery] int imageNumber = 0) =>
            {
                var tempController = new ControllerContext();
                var image = await images.GetProductImageByName(productId);
                return Results.File(image.Data, image.ContentType, image.FileName);
            })
            .WithName("GetProductImageByName");

        app.MapGet("api/v1/image", async ([FromQuery] int productId,
                IOptions<ConnectionOptions> options,
                [FromServices] IImageService images, [FromQuery] int imageNumber = 0) =>
            {
                Console.WriteLine("I got an productId: {0}", productId);
                Console.WriteLine("my connection string is : {0}",
                    options.Value.ConnectionUri);
                // get picture from db and return
                return await images.GetProductImage(productId, imageNumber);
            })
            .WithName("GetProductImagesMetadata");

        app.MapPost("api/v1/image", async ([FromForm] IFormFile file,
            [FromQuery] int productId,
            [FromServices] IImageService images) =>
        {
            if (file is null)
            {
                return "No file provided";
            }

            await images.AddProductImage(productId, file);
            return "File saved";
        }).DisableAntiforgery();

        app.Run();
    }
}
