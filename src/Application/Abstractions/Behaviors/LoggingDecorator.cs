using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogMessages.ProcessingCommand(logger,commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogMessages.CompletedCommand(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogMessages.CompletedCommandError(logger, commandName);
                }
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogMessages.ProcessingCommand(logger,commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogMessages.CompletedCommand(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogMessages.CompletedCommandError(logger, commandName);
                }
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            LogMessages.ProcessingQuery(logger, queryName);
            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                LogMessages.CompletedQuery(logger, queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogMessages.CompletedQueryError(logger, queryName);
                }
            }

            return result;
        }
    }
}
