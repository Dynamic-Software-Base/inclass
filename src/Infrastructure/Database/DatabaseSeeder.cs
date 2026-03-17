using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;


/// <summary>
/// Orchestrates all registered <see cref="ISeeder"/> implementations.
/// Resolves them from DI, sorts by <see cref="ISeeder.Order"/>, and runs each in sequence.
/// Any failure stops the chain and surfaces the exception — seed failures should
/// prevent startup in production rather than silently produce a broken state.
/// </summary>
public sealed class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();

        IEnumerable<ISeeder> seeders = scope.ServiceProvider.GetServices<ISeeder>().OrderBy(o => o.Order);

        foreach (ISeeder seeder in seeders)
        {
            string name  = seeder.GetType().Name;
            _logger.LogInformation("Running seeder: {Seeder}", name);
            try
            {
                await seeder.SeedAsync(cancellationToken);
                _logger.LogInformation("Seeder: {Seeder} completed", name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Seeder failed: {Seeder}", name);
                throw new Exception(e.Message); // Fail fast — a broken seed state is worse than no startup
            }
        }
    }
}
