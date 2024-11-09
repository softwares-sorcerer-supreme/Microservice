using AuthService.Application.Constants;
using AuthService.Application.Options;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure;

public static class Config
{
    // public static IEnumerable<IdentityResource> IdentityResources =>
    // [
    //     new IdentityResources.OpenId(),
    //     new IdentityResources.Profile(),
    //     // new IdentityResource
    //     // {
    //     //     Name = "roles",
    //     //     UserClaims = new[] { "role" }
    //     // }
    // ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope(ApiScopeConstant.ProductApiRead),
        new ApiScope(ApiScopeConstant.ProductApiWrite),
        new ApiScope(ApiScopeConstant.CartApiRead),
        new ApiScope(ApiScopeConstant.CartApiWrite)
    ];

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        var clientOptions = new ClientOptions();
        configuration.GetSection(ClientOptions.OptionName).Bind(clientOptions);

        var clients = new List<Client>
        (
            [
                // client credentials flow client
                new Client
                {
                    ClientId = clientOptions.UserCredentials.ClientId,
                    ClientName = clientOptions.UserCredentials.ClientName,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret(clientOptions.UserCredentials.ClientSecret.Sha256())
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = clientOptions.UserCredentials.AccessTokenLifetime, //86400,
                    // IdentityTokenLifetime = 120, //86400,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AllowOfflineAccess = true,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = clientOptions.UserCredentials.RefreshTokenLifetime, // 30 days
                    // SlidingRefreshTokenLifetime = 30, // it will be reset every you refresh the token
                    // AlwaysSendClientClaims = true,
                    Enabled = true,
                    AllowedScopes =
                    {
                        ApiScopeConstant.ProductApiWrite,
                        ApiScopeConstant.ProductApiRead,
                        ApiScopeConstant.CartApiRead,
                        ApiScopeConstant.CartApiWrite,
                        ApiScopeConstant.OfflineAccess
                    }
                },

                // interactive client using code flow + pkce
                // new Client
                // {
                //     ClientId = "interactive",
                //     ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                //
                //     AllowedGrantTypes = GrantTypes.Code,
                //
                //     RedirectUris = { "https://localhost:5000/signin-oidc" },
                //     FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",
                //     PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
                //
                //     AllowOfflineAccess = true,
                //     AllowedScopes =
                //     {
                //         IdentityServerConstants.StandardScopes.OpenId,
                //         IdentityServerConstants.StandardScopes.Profile,
                //         ApiScopeConstant.ProductApiRead,
                //         ApiScopeConstant.ProductApiWrite,
                //         ApiScopeConstant.CartApiRead,
                //         ApiScopeConstant.CartApiWrite,
                //     },
                //
                //     AllowedCorsOrigins = { "https://localhost:5001" }
                // }
            ]
        );

        return clients;
    }
}