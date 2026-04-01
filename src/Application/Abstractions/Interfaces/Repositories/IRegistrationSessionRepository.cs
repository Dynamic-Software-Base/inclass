using Domain.EducationalSystem.Entities;
using Domain.Registrations;
using Domain.Schools;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IRegistrationSessionRepository
{
    Task AddAsync(RegistrationSession session, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<RegistrationSession> sessions, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        AcademicYear academicYear,
        CancellationToken cancellationToken = default);
    Task<RegistrationSession?> GetByIdAsync(
        RegistrationSessionId id,
        CancellationToken cancellationToken = default);
}
