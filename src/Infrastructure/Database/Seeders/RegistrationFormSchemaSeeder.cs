using System.Reflection;
using System.Text.Json;
using Contract.InClass.Response.Registration.Schemas;
using Domain.EducationalSystem.Entities;
using Domain.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Seeders;

public sealed class RegistrationFormSchemaSeeder : ISeeder
{   private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    // Runs after GradeDefinition seeder — grades must exist first
    public int Order => 20;

    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegistrationFormSchemaSeeder> _logger;

    // System user ID used as CreatedBy for seeded records
    private static readonly UserId SystemUserId = new(Guid.Empty);

    public RegistrationFormSchemaSeeder(
        ApplicationDbContext context,
        ILogger<RegistrationFormSchemaSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // 1. Load all grade definitions from DB — need Code → Id mapping
        List<GradeDefinition> grades = await _context.GradeDefinitions
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (grades.Count == 0)
        {
            _logger.LogWarning("No grade definitions found — skipping schema seeding.");
            return;
        }

        // 2. Read all embedded JSON files
        IEnumerable<(string FileName, string Content)> schemaFiles = ReadEmbeddedSchemaFiles();

        foreach ((string? fileName, string? content) in schemaFiles)
        {
            // 3. Parse the JSON
            FormSchemaResponse? parsed = JsonSerializer.Deserialize<FormSchemaResponse>(
                content,
                JsonOptions);

            if (parsed is null)
            {
                _logger.LogWarning("Failed to parse schema file: {File}", fileName);
                continue;
            }

            // 4. Find matching GradeDefinition by Code
            GradeDefinition? grade = grades.FirstOrDefault(g =>
                g.Code.Equals(parsed.SchemaCode, StringComparison.OrdinalIgnoreCase));

            if (grade is null)
            {
                _logger.LogWarning(
                    "No GradeDefinition found for schemaCode: {Code} in file: {File}",
                    parsed.SchemaCode, fileName);
                continue;
            }

            var gradeId = new GradeDefinitionId(grade.Id.Value);

            // 5. Check if default schema already exists for this grade
            RegistrationFormSchema? existing = await _context.RegistrationFormSchemas
                .FirstOrDefaultAsync(
                    s => s.SchoolId == null && s.GradeDefinitionId == gradeId,
                    cancellationToken);

            if (existing is null)
            {
                // First time — insert
                ErrorOr<RegistrationFormSchema> createResult = RegistrationFormSchema.CreateDefault(
                    RegistrationFormSchemaId.New(),
                    gradeId,
                    content,
                    $"v{parsed.Version}",
                    SystemUserId);

                if (createResult.IsError)
                {
                    _logger.LogError(
                        "Failed to create default schema for grade {Grade}: {Errors}",
                        grade.Code,
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    continue;
                }

                await _context.RegistrationFormSchemas.AddAsync(
                    createResult.Value, cancellationToken);

                _logger.LogInformation(
                    "Inserted default schema for grade: {Grade}", grade.Code);
            }
            else if (parsed.Version > existing.Version)
            {
                // File version is newer — update the default
                // Never touches school-customized schemas (those have SchoolId != null)
                existing.UpdateSchema(content, SystemUserId);

                _logger.LogInformation(
                    "Updated default schema for grade: {Grade} to version {Version}",
                    grade.Code, parsed.Version);
            }
            else
            {
                _logger.LogInformation(
                    "Schema for grade {Grade} is up to date — skipping.", grade.Code);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<(string FileName, string Content)> ReadEmbeddedSchemaFiles()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Embedded resource names follow namespace convention:
        // Infrastructure.Database.Seeds.Schemas.7eme.json
        IEnumerable<string> resourceNames = assembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                        && n.Contains("Schemas"));
        string[] allResources = assembly.GetManifestResourceNames();

        foreach (string res in allResources)
        {
            Console.WriteLine(res);
        }
        foreach (string name in resourceNames)
        {
            using Stream? stream = assembly.GetManifestResourceStream(name);
            if (stream is null) { continue; }

            using var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            yield return (name, content);
        }
    }
}
