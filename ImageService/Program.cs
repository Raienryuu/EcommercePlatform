using Common;
using ImageService;
using ImageService.Models;
using ImageService.Services;
using ImageService.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
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
  _ = app.UseCors();
}
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler();
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
    static async Task<Results<FileContentHttpResult, ProblemHttpResult>> (
      [FromQuery] Guid productId,
      [FromServices] IImageService images,
      [FromQuery] int imageNumber = 0
    ) =>
    {
      if (productId == Guid.Empty)
      {
        return TypedResults.Problem("Product Id is required", statusCode: 400);
      }
      var imageResult = await images.GetProductImageAsync(productId, imageNumber);
      return imageResult.IsSuccess
        ? TypedResults.File(imageResult.Value.Data, imageResult.Value.ContentType, imageResult.Value.Name)
        : TypedResults.Problem(imageResult.ErrorMessage, statusCode: imageResult.StatusCode);
    }
  )
  .WithName("GetProductImages");

app.MapGet(
    "api/v1/imageMetadata",
    static async Task<Results<Ok<ProductImagesMetadata>, ProblemHttpResult>> (
      [FromQuery] Guid productId,
      IProductImagesMetadataService metadataService
    ) =>
    {
      if (productId == Guid.Empty)
      {
        return TypedResults.Problem("Product Id is required", statusCode: 400);
      }
      var metadata = await metadataService.GetProductImagesMetadataAsync(productId);
      if (metadata.IsSuccess)
      {
        return TypedResults.Ok(metadata.Value);
      }
      return TypedResults.Problem(metadata.ErrorMessage, statusCode: metadata.StatusCode);
    }
  )
  .WithName("GetImageMetadata");

app.MapPost(
    "api/v1/image",
    static async Task<Results<CreatedAtRoute, ProblemHttpResult>> (
      [FromForm] IFormFile file,
      [FromQuery] Guid productId,
      [FromServices] IImageService images
    ) =>
    {
      if (productId == Guid.Empty)
      {
        return TypedResults.Problem("Product Id is required", statusCode: 400);
      }
      if (file.Length == 0)
      {
        return TypedResults.Problem("No file provided", statusCode: 400);
      }

      var result = await images.AddProductImageAsync(productId, file);
      if (result.IsSuccess)
      {
        return TypedResults.CreatedAtRoute("api/v1/image", new { productId, imageNumber = result.Value });
      }
      return TypedResults.Problem(result.ErrorMessage, statusCode: result.StatusCode);
    }
  )
  .DisableAntiforgery()
  .WithName("AddProductImage");

app.Run();
