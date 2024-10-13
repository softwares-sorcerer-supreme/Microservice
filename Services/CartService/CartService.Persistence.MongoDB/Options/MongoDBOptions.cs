namespace CartService.Persistence.MongoDB.Options;

public class MongoDBOptions
{
    public const string OptionName = "MongoDBSettings";
    
    public string DatabaseName { get; set; }
    public string HostName { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}