using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.StartupRegistration;

public static class CustomAuthenticationRegistration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        const string authenticationProviderKey = "IdentityApiKey";
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(authenticationProviderKey, options =>
        {
            // it's recommended to check the type header to avoid "JWT confusion" attacks
            options.TokenValidationParameters.ValidTypes = ["at+jwt"];

            options.Authority = configuration.GetValue<string>("AuthServer"); // Set this to your IdentityServer4 URL
            options.RequireHttpsMetadata = false; // Set to true in production environments
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, // Validate that the token was intended for your API
                ValidateIssuer = true, // Validate that the token was issued by your IdentityServer4
                ValidateIssuerSigningKey = true, // Validate the token signature with the signing key
                RequireExpirationTime = true, // Ensure the token has an expiration
                ValidateLifetime = true // Ensure the token is still valid
            };
        });

        return services;
    }
}