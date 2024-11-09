using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;
using FluentValidation;

namespace CartService.Application.Validators.CartItemValidators;

public class AddProductToCartValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddProductToCartValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull()
            .WithMessage("Invalid data value");

        RuleFor(x => x.Payload.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}