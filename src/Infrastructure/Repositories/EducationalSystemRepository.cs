using Application.Abstractions.Interfaces.Repositories;
using Contract.InClass.Response.School.EducationalSystem;
using Domain.EducationalSystem.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Repositories;

public sealed class EducationalSystemRepository : IEducationalSystemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EducationalSystemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ── ExistsAsync ──────────────────────────────────────────────────────────

    public async Task<bool> ExistsAsync(
        EducationalSystemId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalSystems
            .AsNoTracking()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    public async Task<List<EducationalSystemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalSystems
            .AsNoTracking()
            .OrderBy(e => e.Code)
            .Select(e => new EducationalSystemDto(
                e.Id.Value,
                e.Code,
                e.Name_Fr,
                e.Name_Ar))
            .ToListAsync(cancellationToken);
    }

    // ── GetCyclesWithGradesAsync ─────────────────────────────────────────────

    public async Task<List<GradeCycleWithGradesDto>> GetCyclesWithGradesAsync(
        EducationalSystemId educationalSystemId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.GradeCycleDefinitions
            .AsNoTracking()
            .Where(c => c.EducationalSystemId == educationalSystemId)
            .OrderBy(c => c.SortOrder)
            .Select(c => new GradeCycleWithGradesDto(
                c.Id.Value,
                c.Code,
                c.Name_Fr,
                c.Name_Ar,
                c.SortOrder,
                c.Grades
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.SortOrder)
                    .Select(g => new GradeDefinitionDto(
                        g.Id.Value,
                        g.Code,
                        g.Name_Fr,
                        g.Name_Ar,
                        g.SortOrder,
                        g.IsActive))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    // ── GetGradeDefinitionsWithCyclesAsync ───────────────────────────────────

    public async Task<List<(GradeDefinition Grade, GradeCycleDefinition Cycle)>> GetGradeDefinitionsWithCyclesAsync(
        IReadOnlyList<GradeDefinitionId> ids,
        CancellationToken cancellationToken = default)
    {
        // Single query — join grade_definitions to grade_cycle_definitions.
        // We need tracked entities here (no AsNoTracking) because the
        // handler passes them into school.AddSupportedGrade which reads
        // their properties, not because we intend to modify them.
        // They are read-only config data so tracking overhead is negligible.
        List<GradeDefinition> grades = await _dbContext.GradeDefinitions
            .Where(g => ids.Contains(g.Id) && g.IsActive)
            .Include(g => g.Cycle)              // loads GradeCycleDefinition
            .ToListAsync(cancellationToken);

        return grades
            .Select(g => (g, g.Cycle))
            .ToList();
    }
}
