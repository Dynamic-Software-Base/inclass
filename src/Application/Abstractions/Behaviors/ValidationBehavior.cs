using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
where TRequest : IRequest
where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            ValidationResult[] validationResult = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            Error[] errors = validationResult.SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(error => Error.Validation(error.PropertyName , error.ErrorMessage))
                .ToArray();

            if (errors.Length > 0)
            {
                return (dynamic)errors;
            }
        }
        return await next(cancellationToken);

    }
}
