namespace Caching.Options;

public class RedisOptions
{
    public static string OptionName = "Redis";
    public string Host { get; set; }
    public string Port { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool IsSSL { get; set; } = false;
}