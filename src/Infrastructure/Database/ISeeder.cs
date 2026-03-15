namespace Infrastructure.Database;

public interface ISeeder
{
    /// <summary>
    /// Logical order of execution. Lower runs first.
    /// Use gaps (10, 20, 30) so future seeders can be inserted without renumbering.
    /// </summary>
    int Order { get; }
    Task SeedAsync(CancellationToken cancellationToken = default);
}
