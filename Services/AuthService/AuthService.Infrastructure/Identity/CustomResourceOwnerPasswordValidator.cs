using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Identity;

public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly ILogger<CustomResourceOwnerPasswordValidator> _logger;

    public CustomResourceOwnerPasswordValidator(ILogger<CustomResourceOwnerPasswordValidator> logger)
    {
        _logger = logger;
    }

    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        const string functionName = $"{nameof(CustomResourceOwnerPasswordValidator)} - {nameof(ValidateAsync)} => ";
        _logger.LogInformation($"{functionName} UserName = {context.UserName}, ClientId = {context.Request.ClientId}");

        try
        {
            context.Result = new GrantValidationResult(context.UserName, GrantType.ResourceOwnerPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error: Message = {ex.Message}");
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
        }

        return Task.CompletedTask;
    }
}