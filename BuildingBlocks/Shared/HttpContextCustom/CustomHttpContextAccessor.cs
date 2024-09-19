using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Shared.HttpContextCustom;

public interface ICustomHttpContextAccessor
{
    string GetCurrentUserId();
    bool IsUserAuthenticated();
}

public class CustomHttpContextAccessor : ICustomHttpContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? GetHttpContext() => _httpContextAccessor.HttpContext;

    public string GetCurrentUserId() =>
        _httpContextAccessor.HttpContext?.User.FindFirst(JwtClaimTypes.Subject)?.Value ??
        GetCurrentUserIdFromAccessToken();

    public bool IsUserAuthenticated() => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private string GetCurrentUserIdFromAccessToken()
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(GetAccessToken());
            return jwt.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    private string GetAccessToken()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return string.Empty;
        }

        return _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()
            .Replace($"{JwtBearerDefaults.AuthenticationScheme} ", "") ?? string.Empty;
    }
}