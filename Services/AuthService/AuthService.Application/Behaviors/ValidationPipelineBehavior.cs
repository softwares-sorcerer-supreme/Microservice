using FluentValidation;
using MediatR;
using Shared.Validation;
using ValidationException = Shared.Validation.ValidationException;

namespace AuthService.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this._validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationErrors = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Select(x => new ValidationError
            {
                Field = x.PropertyName,
                ErrorMessage = x.ErrorMessage,
                ErrorMessageCode = x.ErrorCode
            })
            .ToList();

        if (validationErrors.Any())
        {
            var validationResultModel = new ValidationResultModel
            {
                Errors = validationErrors
            };
            var validationException = new ValidationException(validationResultModel);
            throw validationException;
        }

        return next();
    }
}