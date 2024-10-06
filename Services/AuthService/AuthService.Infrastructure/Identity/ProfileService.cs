using System.Security.Claims;
using AuthService.Persistence.Identity;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity;

public class ProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService
    (
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        UserManager<ApplicationUser> userManager
    )
    {
        _claimsFactory = claimsFactory;
        _userManager = userManager;
    }
    
    public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject?.GetSubjectId();
        if (sub == null) throw new Exception("No sub claim present");

        await GetProfileDataAsync(context, sub);
    }

    protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, string subjectId)
    {
        var user = await _userManager.FindByIdAsync(subjectId);
        if (user != null)
        {
            await GetProfileDataAsync(context, user);
        }
    }

    protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {
        var principal = await GetUserClaimsAsync(user);
        
        context.AddRequestedClaims(principal.Claims);

        #region Custom claims
        
        var claims = principal.Claims.ToList()
            .Where(claim => context.RequestedClaimTypes.Contains(claim.Type))
            .ToList();
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        context.IssuedClaims = claims;

        #endregion
    }

    protected virtual async Task<ClaimsPrincipal> GetUserClaimsAsync(ApplicationUser user)
    {
        var principal = await _claimsFactory.CreateAsync(user);
        if (principal == null)
        {
            throw new Exception("ClaimsFactory failed to create a principal");
        }

        return principal;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}