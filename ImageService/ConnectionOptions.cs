namespace ImageService;

public class ConnectionOptions
{
    public static string Key { get; set; } = "Mongo";
    public required string ConnectionUri { get; set; }
    public required string DatabaseName { get; set; }
}
