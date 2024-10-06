using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;
using MediatR;

namespace AuthService.Application.UseCases.v1.Commands.RenewToken;

public class RenewTokenCommand : IRequest<RefreshTokenResponse>
{
    public RefreshTokenRequest Payload { get; set; }
    
    public RenewTokenCommand(RefreshTokenRequest payload)
    {
        Payload = payload;
    }
}