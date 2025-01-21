using ImageService.Models;
using ImageService.Services;
using ImageService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace ImageService;

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
        builder.Services.AddSingleton<IProductImagesMetadataService, MongoProductImagesMetadataService>();
        builder.Services.AddSingleton<INameFormatter, NameFormatter>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        BsonClassMap.RegisterClassMap<ProductImagesMetadata>(map =>
                {
                    map.MapCreator(p => new ProductImagesMetadata(p.ProductId, p.StoredImages, new MetadataAvaiable()));

                    map.AutoMap();
                    map.UnmapProperty(p => p.MetadataState);
                });

        app.UseHttpsRedirection();
        app.MapGet("api/v1/image", async ([FromQuery] int productId,
            [FromServices] IImageService images, [FromQuery] int imageNumber = 0) =>
        {
            var image = await images.GetProductImageAsync(productId, imageNumber);
            if (image is null)
            {
                return Results.NotFound("No image found");
            }
            return Results.File(image.Data, image.ContentType, image.Name);
        }).WithName("GetProductImages");

        app.MapPost("api/v1/image", async ([FromForm] IFormFile file,
                [FromQuery] int productId,
                [FromServices] IImageService images) =>
            {
                if (file is null)
                {
                    return "No file provided";
                }

                await images.AddProductImageAsync(productId, file);
                return "File saved";
            }).DisableAntiforgery()
            .WithName("AddProductImage");

        app.Run();
    }
}
