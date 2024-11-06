namespace AuthService.Application.Models.Requests;

public record RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}