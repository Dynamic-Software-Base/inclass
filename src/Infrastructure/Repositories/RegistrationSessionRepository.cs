using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Repositories;

public class RegistrationSessionRepository : IRegistrationSessionRepository
{
    private readonly ApplicationDbContext dbContext;

    public RegistrationSessionRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(RegistrationSession session, CancellationToken cancellationToken = default)
    {
      await dbContext.RegistrationSessions.AddAsync(session, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<RegistrationSession> sessions, CancellationToken cancellationToken = default)
    {
        await dbContext.RegistrationSessions.AddRangeAsync(sessions, cancellationToken);
    }

    public async Task<bool> ExistsAsync(SchoolId schoolId, GradeDefinitionId gradeDefinitionId, AcademicYear academicYear,
        CancellationToken cancellationToken = default)
    {
      return await dbContext.RegistrationSessions.AnyAsync(c => c.SchoolId == schoolId
                                                                && gradeDefinitionId ==  c.GradeDefinitionId
                                                                && c.AcademicYear.Value == academicYear.Value
                                                                ,cancellationToken);
    }

    public async Task<RegistrationSession?> GetByIdAsync(
        RegistrationSessionId id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.RegistrationSessions
            .FindAsync([id], cancellationToken);
    }
}
