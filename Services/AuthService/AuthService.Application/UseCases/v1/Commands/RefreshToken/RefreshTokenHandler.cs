using AuthService.Application.Abstraction.Interfaces;
using AuthService.Application.Models.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<RefreshTokenHandler> _logger;
    public RefreshTokenHandler(IIdentityService identityService, ILogger<RefreshTokenHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(RefreshTokenHandler)} =>";
        _logger.LogInformation($"{functionName}");
        var response = new RefreshTokenResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };
        var payload = request.Payload;
        try
        {
            var refreshToken = payload.RefreshToken;
            var tokenResponse = await _identityService.RefreshTokenAsync(refreshToken);
            // if (tokenResponse.HasError)
            // {
            //     _logger.LogWarning($"{functionName} Error while refreshing token");
            //     response.Status = tokenResponse.Status;
            //     response.ErrorMessageCode = tokenResponse.ErrorMessageCode;
            //     return response;
            // }
            // var data = new RefreshTokenData
            // {
            //     AccessToken = tokenResponse.Data.AccessToken,
            //     RefreshToken = tokenResponse.Data.RefreshToken
            // };
            // response.Data = data;
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return response;
        }
    }
}