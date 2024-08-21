namespace BusinessLayer.MongoDb;

public class MongoDbConnectionSettings
{
    public string SerperResponseCollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}