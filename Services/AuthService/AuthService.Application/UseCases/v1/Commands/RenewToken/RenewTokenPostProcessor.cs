using AuthService.Application.Constants;
using AuthService.Application.Models.Responses;
using AuthService.Application.UseCases.v1.Commands.Login;
using Caching.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.RenewToken;

public class RenewTokenPostProcessor : IRequestPostProcessor<LoginCommand, LoginResponse>
{
    private readonly IRedisService _redisService;
    private readonly ILogger<RenewTokenPostProcessor> _logger;
    
    public RenewTokenPostProcessor
    (
        IRedisService redisService,
        ILogger<RenewTokenPostProcessor> logger
    )
    {
        _redisService = redisService;
        _logger = logger;
    }
    
    public async Task Process(LoginCommand request, LoginResponse response, CancellationToken cancellationToken)
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
            await _redisService.HashSetAsync(AuthCacheKeysConst.RefreshTokenStoresKey, postProcessorData.UserId, postProcessorData.RefreshToken);
            await _redisService.KeyExpireAsync(AuthCacheKeysConst.RefreshTokenStoresKey, DateTime.UtcNow.AddMilliseconds(14400));
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"{functionName} has Error: {ex.Message}");
        }
    }
}