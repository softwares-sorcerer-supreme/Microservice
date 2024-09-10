using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;
using MediatR;

namespace AuthService.Application.UseCases.v1.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public RefreshTokenRequest Payload { get; set; }
    
    public RefreshTokenCommand(RefreshTokenRequest payload)
    {
        Payload = payload;
    }
}