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
}