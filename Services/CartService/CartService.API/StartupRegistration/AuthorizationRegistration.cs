using CartService.API.Authorizations;
using Microsoft.AspNetCore.Authorization;
using Shared.Constants;

namespace CartService.API.StartupRegistration;

public static class AuthorizationRegistration
{
    public static IServiceCollection AddAuthorizationRegistration(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy",
                policyBuilder =>
                {
                    policyBuilder.AddRequirements(new RolePolicyRequirement(RoleConst.Admin)); 
                })
            .AddPolicy("UserPolicy",
                policyBuilder =>
                {
                    policyBuilder.AddRequirements(new RolePolicyRequirement(RoleConst.Guest)); 
                });
        
        services.AddSingleton<IAuthorizationHandler, RolePolicyHandler>();

        return services;
    }
}