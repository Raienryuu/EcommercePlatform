using MongoDB.Bson;

namespace ImageService.Models;

public class Image
{
  public ObjectId Id { get; set; }
  public required string ContentType { get; init; }

  public required long Length { get; init; }

  public required string Name { get; init; }

  public required byte[] Data { get; init; }
  public required uint Width { get; init; }
  //public  Orientation { get; init; }
}
