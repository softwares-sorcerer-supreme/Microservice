using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProductService.Infrastructure.Options;

namespace ProductService.Infrastructure.StartupRegistration;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.OptionName).Bind(jwtOptions);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            // it's recommended to check the type header to avoid "JWT confusion" attacks
            options.TokenValidationParameters.ValidTypes = ["at+jwt"];

            options.Authority = jwtOptions.Authority; // Set this to your IdentityServer4 URL
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

            // SignalR
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs"))) // for me my hub endpoint is ConnectionHub
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}