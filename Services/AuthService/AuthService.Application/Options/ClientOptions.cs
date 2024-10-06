namespace AuthService.Application.Options;

public class ClientOptions
{
    public static string OptionName = "Clients";
    public UserCredential UserCredentials { get; set; }
}

public class UserCredential
{
    public string ClientName { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public int AccessTokenLifetime { get; set; }
    public int RefreshTokenLifetime { get; set; }
}