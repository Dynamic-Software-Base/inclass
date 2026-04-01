using Domain.Registrations;
using Domain.Schools;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IStudentApplicationRepository
{
    Task AddAsync(StudentApplication application, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        RegistrationSessionId sessionId,
        string firstName,
        string lastName,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<StudentApplication?> GetByIdAsync(
        StudentApplicationId id,
        CancellationToken cancellationToken = default);
}
