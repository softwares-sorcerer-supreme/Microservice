namespace AuthService.Application.Models.Requests;

public record LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}