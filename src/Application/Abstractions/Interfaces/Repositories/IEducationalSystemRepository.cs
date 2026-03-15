using Contract.InClass.Response.School.EducationalSystem;
using Domain.EducationalSystem.Entities;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IEducationalSystemRepository
{
    /// <summary>
    /// Checks whether an EducationalSystem with the given ID exists.
    /// Used in CreateSchoolCommandHandler to validate the incoming EducationalSystemId.
    /// </summary>
    Task<bool> ExistsAsync(
        EducationalSystemId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all active EducationalSystems as lightweight DTOs for the
    /// "pick your school system" dropdown on the create-school form.
    /// </summary>
    Task<List<EducationalSystemDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all GradeCycleDefinitions (with their GradeDefinition children)
    /// for a given EducationalSystem. Used to populate the curriculum picker UI
    /// so the owner can select cycles or individual grades.
    /// </summary>
    Task<List<GradeCycleWithGradesDto>> GetCyclesWithGradesAsync(
        EducationalSystemId educationalSystemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads GradeDefinition + its parent GradeCycleDefinition for a list of IDs.
    /// Used in CreateSchoolCommandHandler to call school.AddSupportedGrade(grade, cycle).
    /// Returns only active grades. If any ID is not found or inactive, the count
    /// will not match — the handler uses this to detect invalid input.
    /// </summary>
    Task<List<(GradeDefinition Grade, GradeCycleDefinition Cycle)>> GetGradeDefinitionsWithCyclesAsync(
        IReadOnlyList<GradeDefinitionId> ids,
        CancellationToken cancellationToken = default);
}
