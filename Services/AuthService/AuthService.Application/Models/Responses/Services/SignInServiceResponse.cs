using AuthService.Application.Models.Dtos;
using Shared.Models.Response;

namespace AuthService.Application.Models.Responses.Services;

public record SignInServiceResponse : ErrorServiceResponse
{
    public SignInServiceData Data { get; set; }
}

public record SignInServiceData
{
    public UserDto User { get; set; }
}