using AuthService.Application.UseCases.v1.Commands.Login;
using FluentValidation;

namespace AuthService.Application.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .WithMessage("Username is required");
        // .EmailAddress()
        // .WithMessage("Email is not valid");

        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");
    }
}