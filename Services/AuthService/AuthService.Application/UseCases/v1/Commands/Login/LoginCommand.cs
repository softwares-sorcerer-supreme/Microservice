using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;
using MediatR;

namespace AuthService.Application.UseCases.v1.Commands.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public LoginRequest Payload { get; set; }

    public LoginCommand(LoginRequest payload)
    {
        Payload = payload;
    }
}