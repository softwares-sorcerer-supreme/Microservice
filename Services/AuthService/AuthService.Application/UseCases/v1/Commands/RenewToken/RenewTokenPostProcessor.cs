using AuthService.Application.Constants;
using AuthService.Application.Models.Responses;
using AuthService.Application.Options;
using AuthService.Application.UseCases.v1.Commands.Login;
using Caching.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.CommonExtension;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.RenewToken;

public class RenewTokenPostProcessor : IRequestPostProcessor<RenewTokenCommand, RefreshTokenResponse>
{
    private readonly IRedisService _redisService;
    private readonly ILogger<RenewTokenPostProcessor> _logger;
    private readonly ClientOptions _clientOptions;
    
    public RenewTokenPostProcessor
    (
        IRedisService redisService,
        ILogger<RenewTokenPostProcessor> logger,
        IOptions<ClientOptions> clientOptions
    )
    {
        _redisService = redisService;
        _logger = logger;
        _clientOptions = clientOptions.Value;
    }
    
    public async Task Process(RenewTokenCommand request, RefreshTokenResponse response, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(RenewTokenPostProcessor)} =>";
        _logger.LogInformation($"{functionName}");
        
        try
        {
            if(response.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} Error while logging in");
                return;
            }

            var postProcessorData = response.Data;
            await _redisService.HashSetAsync
            (
                AuthCacheKeysConst.RefreshTokenStoresKey,
                postProcessorData.UserId,
                postProcessorData.RefreshToken,
                TimeSpan.FromSeconds(_clientOptions.UserCredentials.RefreshTokenLifetime)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"{functionName} has Error: {ex.Message}");
        }
    }
}