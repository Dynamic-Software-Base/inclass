using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Behaviors;


public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

#pragma warning disable S6672
    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
#pragma warning restore S6672
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
#pragma warning disable S2139
        catch (Exception ex)
#pragma warning restore S2139
        {
            string requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "CleanArchitecture Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
            throw;
        }
    }
}
