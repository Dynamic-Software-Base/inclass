namespace Web.Api.Common.Logger;

internal static partial class LogMessages
{

    [LoggerMessage(Level = LogLevel.Information, Message = "[MIGRATION] No migration found.")]
    internal static partial void NoMigrationFound(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information , Message = "[MIGRATION] Migrating database...")]
    internal static partial void Migrating(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "[MIGRATION]-[PENDING] => {Name}")]
    internal static partial void LogPendingMigrationNames(ILogger logger , string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "[MIGRATION] Completed")]
    internal static partial void MigratingCompleted(ILogger logger );
}
