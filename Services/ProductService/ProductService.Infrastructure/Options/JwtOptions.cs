namespace ProductService.Infrastructure.Options;

public class JwtOptions
{
    public const string OptionName = "JwtSettings";
    
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 60;
}