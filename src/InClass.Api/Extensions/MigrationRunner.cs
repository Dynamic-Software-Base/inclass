using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Web.Api.Common.Logger;

namespace Web.Api.Extensions;

public static class MigrationRunner
{

    public static async Task RunMigrationAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        DbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(MigrationRunner));
        IEnumerable<string> pending = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pending.Any())
        {
            LogMessages.NoMigrationFound(logger);
            return;
        }
        LogMessages.Migrating(logger);
        foreach (string m in pending)
        {
           LogMessages.LogPendingMigrationNames(logger, m);
        }

        await dbContext.Database.MigrateAsync();
       LogMessages.MigratingCompleted(logger);


    }
}
