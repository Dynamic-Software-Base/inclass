using Microsoft.Extensions.Logging;

namespace Application;

internal static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Processing command {Command}")]
    public static partial void ProcessingCommand(ILogger logger, string command);

    [LoggerMessage(Level = LogLevel.Information, Message = "Completed command {Command}")]
    public static partial void CompletedCommand(ILogger logger, string command);

    [LoggerMessage(Level = LogLevel.Error, Message = "Completed command {Command} with error")]
    public static partial void CompletedCommandError(ILogger logger, string command);



    [LoggerMessage(Level = LogLevel.Information, Message = "Processing query {query}")]
    public static partial void ProcessingQuery(ILogger logger, string query);

    [LoggerMessage(Level = LogLevel.Information, Message = "Completed Query {query}")]
    public static partial void CompletedQuery(ILogger logger, string query);

    [LoggerMessage(Level = LogLevel.Error, Message = "Completed Query {query} with error")]
    public static partial void CompletedQueryError(ILogger logger, string query);
}
