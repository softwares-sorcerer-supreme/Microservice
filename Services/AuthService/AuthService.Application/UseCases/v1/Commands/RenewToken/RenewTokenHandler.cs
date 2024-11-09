using AuthService.Application.Abstraction.Interfaces;
using AuthService.Application.Constants;
using AuthService.Application.Models.Responses;
using Caching.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;
using Shared.HttpContextCustom;
using RefreshTokenRequest = IdentityModel.Client.RefreshTokenRequest;

namespace AuthService.Application.UseCases.v1.Commands.RenewToken;

public class RenewTokenHandler : IRequestHandler<RenewTokenCommand, RefreshTokenResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IRedisService _redisService;
    private readonly ILogger<RenewTokenHandler> _logger;
    private readonly Options.ClientOptions _clientOptions;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;

    public RenewTokenHandler
    (
        IIdentityService identityService,
        IRedisService redisService,
        IOptions<Options.ClientOptions> clientOptions,
        ICustomHttpContextAccessor httpContextAccessor,
        ILogger<RenewTokenHandler> logger
    )
    {
        _identityService = identityService;
        _redisService = redisService;
        _clientOptions = clientOptions.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<RefreshTokenResponse> Handle(RenewTokenCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(RenewTokenHandler)} =>";
        _logger.LogInformation($"{functionName}");
        var response = new RefreshTokenResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        var payload = request.Payload;
        try
        {
            var refreshToken = payload.RefreshToken;
            var userId = _httpContextAccessor.GetCurrentUserId();
            var cachedRefreshToken = await _redisService.HashGetAsync<string>(AuthCacheKeysConst.RefreshTokenStoresKey, userId);
            if (string.IsNullOrEmpty(cachedRefreshToken))
            {
                _logger.LogWarning($"{functionName} Refresh token not found in cache");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0009;
                return response;
            }

            if (refreshToken != cachedRefreshToken)
            {
                _logger.LogWarning($"{functionName} Refresh token not match");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0010;
                return response;
            }

            var refreshTokenRequest = new RefreshTokenRequest
            {
                Address = $"https://localhost:5001{IdentityServerEndpointConstant.AuthPath}",
                ClientId = _clientOptions.UserCredentials.ClientId,
                ClientSecret = _clientOptions.UserCredentials.ClientSecret,
                RefreshToken = refreshToken
            };

            var tokenResponse = await _identityService.RefreshTokenAsync(refreshTokenRequest);
            if (!string.IsNullOrEmpty(tokenResponse.Error))
            {
                _logger.LogError($"{functionName} Error while refreshing token. Message => {tokenResponse.Error}, Description => {tokenResponse.ErrorDescription}");
                response.Status = ResponseStatusCode.InternalServerError.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
                return response;
            }

            var data = new RefreshTokenData
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                TokenType = tokenResponse.TokenType,
                UserId = userId
            };

            response.Data = data;
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