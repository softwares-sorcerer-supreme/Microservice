using AuthService.Application.Abstraction.Interfaces;
using AuthService.Application.Models.Dtos;
using AuthService.Application.Models.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace AuthService.Application.UseCases.v1.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(IIdentityService identityService, ILogger<RegisterHandler> logger)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(RegisterHandler)} =>";
        var payload = request.Payload;
        _logger.LogInformation($"{functionName} Email => {payload.Email}");
        var response = new RegisterResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        try
        {
            if (payload.Password != payload.ConfirmPassword)
            {
                _logger.LogWarning($"{functionName} Password and Confirm Password does not match");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0006;
                return response;
            }

            var userDto = new UserDto
            {
                Email = payload.Email,
                FullName = payload.FullName,
                PhoneNumber = payload.PhoneNumber,
                Role = payload.Role,
                IsEmailVerified = false
            };

            var result = await _identityService.RegisterAsync(userDto, payload.Password);

            if (result.HasError)
            {
                _logger.LogWarning($"{functionName} Error while registering");
                response.Status = result.Status;
                response.ErrorMessageCode = result.ErrorMessageCode;
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"{functionName} Error while registering => {e.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return response;
        }
    }
}