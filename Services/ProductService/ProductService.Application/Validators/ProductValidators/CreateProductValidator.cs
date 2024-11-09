using FluentValidation;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;

namespace ProductService.Application.Validators.ProductValidators;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull()
            .WithMessage("Invalid data value");

        RuleFor(x => x.Payload.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Payload.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Payload.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0");
    }
}