using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CartService.API.Authorizations;

public class RolePolicyHandler : AuthorizationHandler<RolePolicyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolePolicyRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == false)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        //getting custom_roles from our claims
        var roleClaims = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value.ToString();

        //check if the claims contains the required roles
        if (roleClaims != null && requirement.Roles != null && roleClaims.Split(",").ToArray().Any(requirement.Roles.Contains))
        {
            //contains the required claims
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        //Unauthorized
        context.Fail();
        return Task.CompletedTask;
    }
}