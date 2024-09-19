using AuthService.Application.Constants;
using AuthService.Application.Models.Responses;
using Caching.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.Login;

public class LoginPostProcessor : IRequestPostProcessor<LoginCommand, LoginResponse>
{
    private readonly IRedisService _redisService;
    private readonly ILogger<LoginPostProcessor> _logger;

    public LoginPostProcessor
    (
        IRedisService redisService,
        ILogger<LoginPostProcessor> logger
    )
    {
        _redisService = redisService;
        _logger = logger;
    }
    
    public async Task Process(LoginCommand request, LoginResponse response, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(LoginPostProcessor)} =>";
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
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"{functionName} has Error: {ex.Message}");
        }
    }
}