using Common;
using ImageService;
using ImageService.Models;
using ImageService.Services;
using ImageService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<ConnectionOptions>(
  builder.Configuration.GetRequiredSection(ConnectionOptions.Key)
);
var objectSerializer = new ObjectSerializer(ObjectSerializer.AllAllowedTypes);
BsonSerializer.RegisterSerializer(objectSerializer);
builder.Services.AddSingleton<IImageService, MongoImageService>();
builder.Services.AddSingleton<IProductImagesMetadataService, MongoProductImagesMetadataService>();
builder.Services.AddSingleton<INameFormatter, NameFormatter>();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

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
  tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter();
});

if (
  builder.Configuration["ASPNETCORE_ENVIRONMENT"] == Environments.Development
  || builder.Configuration["ASPNETCORE_ENVIRONMENT"] == "docker"
)
{
  _ = builder.Services.AddCors(static c =>
  {
    c.AddDefaultPolicy(static p =>
    {
      _ = p.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
  });
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  _ = app.MapOpenApi();
  _ = app.UseCors();
}
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
}
BsonClassMap.RegisterClassMap<ProductImagesMetadata>(static map =>
{
  _ = map.MapCreator(static p => new ProductImagesMetadata(
    p.ProductId,
    p.StoredImages,
    new MetadataAvailable()
  ));

  map.AutoMap();
  map.UnmapProperty(static p => p.MetadataState);
});

/*app.UseHttpsRedirection();*/

app.MapGet(
    "api/v1/image",
    static async (
      [FromQuery] Guid productId,
      [FromServices] IImageService images,
      [FromQuery] int imageNumber = 0
    ) =>
    {
      var image = await images.GetProductImageAsync(productId, imageNumber);
      return image is null
        ? Results.NotFound("No image found")
        : Results.File(image.Data, image.ContentType, image.Name);
    }
  )
  .WithName("GetProductImages");

app.MapGet(
    "api/v1/imageMetadata",
    static async ([FromQuery] Guid productId, IProductImagesMetadataService metadataService) =>
    {
      var metadata = await metadataService.GetProductImagesMetadataAsync(productId);
      return Results.Ok(metadata);
    }
  )
  .WithName("GetImageMetadata");

app.MapPost(
    "api/v1/image",
    static async (
      [FromForm] IFormFile file,
      [FromQuery] Guid productId,
      [FromServices] IImageService images
    ) =>
    {
      if (file is null)
      {
        return Results.BadRequest("No file provided");
      }
      await images.AddProductImageAsync(productId, file);
      return Results.Ok("File saved");
    }
  )
  .DisableAntiforgery()
  .WithName("AddProductImage");

app.Run();
