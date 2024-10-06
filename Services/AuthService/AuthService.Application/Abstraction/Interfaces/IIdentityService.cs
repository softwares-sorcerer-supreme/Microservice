using AuthService.Application.Models.Dtos;
using AuthService.Application.Models.Responses.Services;
using IdentityModel.Client;

namespace AuthService.Application.Abstraction.Interfaces;

public interface IIdentityService
{
    Task<TokenResponse> GetTokenFromCredential(PasswordTokenRequest request, CancellationToken cancellationToken);
    Task<SignInServiceResponse> SignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
    Task<RegisterServiceResponse> RegisterAsync(UserDto user, string password);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}