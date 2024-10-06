using Shared.Models.Response;

namespace AuthService.Application.Models.Responses;

public class RefreshTokenResponse : ErrorResponse
{
    public RefreshTokenData Data { get; set; }
}

public class RefreshTokenData
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    internal string UserId { get; set; }
}