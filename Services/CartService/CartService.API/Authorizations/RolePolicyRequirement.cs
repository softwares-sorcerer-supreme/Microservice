using Microsoft.AspNetCore.Authorization;

namespace CartService.API.Authorizations;

public class RolePolicyRequirement : IAuthorizationRequirement
{
    //holds the array of roles
    public string[]? Roles { get; }
    public RolePolicyRequirement(string? roles)
    {
        Roles = roles?.Split(",").ToArray();
    }
}