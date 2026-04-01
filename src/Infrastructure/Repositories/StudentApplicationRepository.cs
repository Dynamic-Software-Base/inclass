using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Repositories;

internal sealed class StudentApplicationRepository(ApplicationDbContext dbContext)
    : IStudentApplicationRepository
{

    public async Task<bool> ExistsForAcademicYearAsync(
        StudentId studentId,
        SchoolId schoolId,
        AcademicYear academicYear,
        CancellationToken ct)
    {
        return await dbContext.StudentApplications
            .AnyAsync(a =>
                a.StudentId == studentId &&
                a.SchoolId == schoolId &&
                a.AcademicYear == academicYear, ct);
    }

    public async Task AddAsync(
        StudentApplication application,
        CancellationToken cancellationToken = default)
    {
        await dbContext.StudentApplications.AddAsync(application, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        RegistrationSessionId sessionId,
        string firstName,
        string lastName,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudentApplications
            .AsNoTracking()
            .AnyAsync(a =>
                    a.SessionId == sessionId &&
                    a.StudentFirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
                    a.StudentLastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) &&
                    a.Contact.PhoneNumber!.Number.Equals(phoneNumber, StringComparison.OrdinalIgnoreCase),
                cancellationToken);
    }

    public async Task<StudentApplication?> GetByIdAsync(
        StudentApplicationId id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudentApplications
            .FindAsync([id], cancellationToken);
    }
}
