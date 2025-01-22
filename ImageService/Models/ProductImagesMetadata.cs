using System.Diagnostics.CodeAnalysis;
using ImageService.Services.Interfaces;
using MongoDB.Bson;

namespace ImageService.Models;
public class ProductImagesMetadata
{
  public ObjectId Id { get; set; }
  public required int ProductId { get; init; }
  public List<string> StoredImages { get; set; } = [];
  public required IMetadataState MetadataState { get; set; }

  [SetsRequiredMembers]
  public ProductImagesMetadata(int productId, List<string> storedImages, IMetadataState state)
  {
    ProductId = productId;
    StoredImages = storedImages;
    MetadataState = state;
  }
}

public interface IMetadataState
{
  void Apply(IProductImagesMetadataService _metadataService, ProductImagesMetadata p);
}
public class NoMetadataAvailable : IMetadataState
{
  public void Apply(IProductImagesMetadataService _metadataService, ProductImagesMetadata p) => _metadataService.AddNewMetadataAsync(p);
}
public class MetadataAvailable : IMetadataState
{
  public void Apply(IProductImagesMetadataService _metadataService, ProductImagesMetadata p) => _metadataService.UpdateMetadataAsync(p);
}
