using Shared.Models.Response;

namespace AuthService.Application.Models.Responses;

public record LoginResponse : BaseResponse
{
    public LoginData Data { get; set; }
}

public record LoginData
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    internal string UserId { get; set; } = string.Empty;
}