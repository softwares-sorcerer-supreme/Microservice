using AuthService.Application.Models.Dtos;
using Shared.Models.Response;

namespace AuthService.Application.Models.Responses.Services;

public class SignInServiceResponse : ErrorServiceResponse
{
    public SignInServiceData Data { get; set; }
}

public class SignInServiceData
{
    public UserDto User { get; set; }
}