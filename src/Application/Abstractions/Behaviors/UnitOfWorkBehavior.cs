using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Behaviors;

public class UnitOfWorkBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TRequest> _logger;
#pragma warning disable S6672
    public UnitOfWorkBehavior(IServiceProvider serviceProvider, ILogger<TRequest> logger)
#pragma warning restore S6672
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async  Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        if (request is not ICommand)
        {
            _logger.LogInformation("we executing non command ");
            return await next(cancellationToken);
        }


        _logger.LogInformation("we executing command");
        IUnitOfWork unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            TResponse response = await next(cancellationToken);
            _logger.LogInformation("we saved command");
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
