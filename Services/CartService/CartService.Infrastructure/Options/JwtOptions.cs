namespace CartService.Infrastructure.Options;

public class JwtOptions
{
    public const string OptionName = "JwtSettings";
    
    public string Authority { get; set; } = string.Empty;
}