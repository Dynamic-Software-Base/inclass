using Domain.Schools;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Application.Abstractions.Interfaces.Repositories;

public interface ISchoolClassRepository
{
    Task AddAsync(SchoolClass schoolClass, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<SchoolClass> schoolClasses, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        AcademicYear academicYear,
        string name,
        CancellationToken cancellationToken = default);

    Task<SchoolClass?> GetByIdAsync(
        SchoolClassId id,
        CancellationToken cancellationToken = default);
}

