namespace AuthService.Application.Constants;

public readonly struct ApiScopeConstant
{
    public const string OfflineAccess = "offline_access";
    public const string ProductApiWrite = "prodct.api.write";
    public const string ProductApiRead = "prodct.api.read";
    public const string CartApiWrite = "cart.api.write";
    public const string CartApiRead = "cart.api.read";
}

public readonly struct IdentityServerEndpointConstant
{
    public const string AuthPath = "/connect/token";
}