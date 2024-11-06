using Shared.Models.Response;

namespace AuthService.Application.Models.Responses;

public record RefreshTokenResponse : BaseResponse
{
    public RefreshTokenData Data { get; set; }
}

public record RefreshTokenData
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    internal string UserId { get; set; }
}