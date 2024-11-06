namespace AuthService.Application.Models.Requests;

public record RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
}