using ImageService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ImageService.Services;

public class MongoImageService : IImageService
{
    private readonly MongoClient _client;
    private readonly ConnectionOptions _options;

    public MongoImageService(IOptions<ConnectionOptions> connectionOptions)
    {
        _options = connectionOptions.Value;
        _client = new MongoClient(_options.ConnectionUri);
    }

    public async Task AddProductImage(int productId, IFormFile file)
    {
        var collection = _client.GetDatabase("ecommerce")
            .GetCollection<Image>("images");
        byte[] imageBytes = new byte[file.Length];
        var imageBytesStream = new MemoryStream();

        await file.OpenReadStream().ReadAsync(imageBytes);
        var fileAsImage = new Image
        {
            ContentType = file.ContentType,
            ContentDisposition = file.ContentDisposition,
            Length = file.Length,
            Name = file.Name,
            FileName = file.FileName,
            Data = imageBytes,
        };
        await collection.InsertOneAsync(fileAsImage);
    }

    public async Task<IFormFile> GetProductImage(int productId, int imageNumber)
    {
        var collection = _client.GetDatabase("ecommerce")
            .GetCollection<IFormFile>("images");
        var filter = Builders<IFormFile>.Filter.Eq("id", productId);
        var results = await collection.FindAsync(filter);
        return await results.FirstAsync();
    }

    public Task GetProductImagesMetadata()
    {
        throw new NotImplementedException();
    }

    public async Task<Image> GetProductImageByName(string productId)
    {
        var collection = _client.GetDatabase("ecommerce")
            .GetCollection<Image>("images");
        var filter = Builders<Image>.Filter.Eq("Name", productId);
        var results = await collection.FindAsync(filter);
        return results.First();
    }
}
