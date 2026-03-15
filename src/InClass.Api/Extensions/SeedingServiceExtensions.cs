using Infrastructure.Database;
using Infrastructure.Database.Seeders;

namespace Web.Api.Extensions;

public static class SeedingServiceExtensions
{
    /// <summary>
    /// Registers the <see cref="DatabaseSeeder"/> orchestrator and all
    /// <see cref="ISeeder"/> implementations.
    ///
    /// Add new seeders here as the system grows — order is controlled
    /// by each seeder's <see cref="ISeeder.Order"/> property, not registration order.
    /// </summary>
    public static IServiceCollection AddSeedingService(this IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();

        // Register each seeder — DI resolves them all when DatabaseSeeder calls GetServices<ISeeder>()
        services.AddScoped<ISeeder, MoroccanEducationSystemSeeder>();

        // Future seeders registered here:
        // services.AddScoped<ISeeder, RoleSeeder>();
        // services.AddScoped<ISeeder, FrenchMissionEducationSystemSeeder>();
        return services;
    }
}
