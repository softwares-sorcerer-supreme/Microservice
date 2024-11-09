using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;
using MediatR;

namespace AuthService.Application.UseCases.v1.Commands.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public RegisterRequest Payload { get; set; }

    public RegisterCommand(RegisterRequest payload)
    {
        Payload = payload;
    }
}