using AuthService.Application.UseCases.v1.Commands.Register;
using FluentValidation;

namespace AuthService.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Payload.FullName)
            .NotEmpty()
            .WithMessage("Full name is required");

        RuleFor(x => x.Payload.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Length(10)
            .WithMessage("Phone number must be 10 digits long")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number is not valid");
    }
}