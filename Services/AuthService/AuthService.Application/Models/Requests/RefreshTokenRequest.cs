namespace AuthService.Application.Models.Requests;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
}