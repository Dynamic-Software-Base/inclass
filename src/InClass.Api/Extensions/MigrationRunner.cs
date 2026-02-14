using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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
            logger.LogInformation("No migrations found.");
            return;
        }
        logger.LogInformation("Migrating database...");
        foreach (string m in pending)
        {
            logger.LogInformation("    ->{MigrationName}", m);
        }

        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migration complete.");
    }
}
