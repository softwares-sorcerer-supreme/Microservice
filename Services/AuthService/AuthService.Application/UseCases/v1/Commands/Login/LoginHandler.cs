using System.Text.Json;
using AuthService.Application.Abstraction.Interfaces;
using AuthService.Application.Constants;
using AuthService.Application.Models.Responses;
using IdentityModel.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LoginHandler> _logger;
    private readonly Options.ClientOptions _clientOptions;

    public LoginHandler
    (
        IIdentityService identityService,
        ILogger<LoginHandler> logger,
        IOptions<Options.ClientOptions> clientOptions
    )
    {
        _logger = logger;
        _identityService = identityService;
        _clientOptions = clientOptions.Value;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(LoginHandler)} =>";
        _logger.LogInformation($"{functionName} {JsonSerializer.Serialize(request.Payload)}");
        var payload = request.Payload;
        var response = new LoginResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        try
        {
            var scopes = new List<string>
            {
                ApiScopeConstant.OfflineAccess,
                ApiScopeConstant.CartApiRead,
                ApiScopeConstant.CartApiWrite,
                ApiScopeConstant.ProductApiRead,
                ApiScopeConstant.ProductApiWrite
            };

            var signInResult = await _identityService.SignInAsync(payload.Username, payload.Password, false, false);

            if (signInResult.HasError)
            {
                _logger.LogWarning($"{functionName} Error while logging in");
                response.Status = signInResult.Status;
                response.ErrorMessageCode = signInResult.ErrorMessageCode;
                return response;
            }

            var user = signInResult.Data.User;
            var tokenRequest = new PasswordTokenRequest
            {
                Address = $"https://localhost:5001{IdentityServerEndpointConstant.AuthPath}",
                ClientId = _clientOptions.UserCredentials.ClientId,
                ClientSecret = _clientOptions.UserCredentials.ClientSecret,
                Scope = string.Join(" ", scopes),
                UserName = user.Id,
                Password = payload.Password,
            };

            var tokenResponse = await _identityService.GetTokenFromCredential(tokenRequest, cancellationToken);
            if (tokenResponse.IsError)
            {
                _logger.LogError($"{functionName} Error while logging in => {tokenResponse.Error}, Description => {tokenResponse.ErrorDescription}");
                response.Status = ResponseStatusCode.InternalServerError.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
                return response;
            }

            var responseData = new LoginData
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                TokenType = tokenResponse.TokenType,
                ExpiresIn = tokenResponse.ExpiresIn,
                UserId = user.Id
            };

            response.Data = responseData;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has Error: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return response;
        }
    }
}