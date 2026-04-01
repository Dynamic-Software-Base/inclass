using Domain.Students;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdentityKeyAsync(
        string identityKey,
        CancellationToken cancellationToken = default);
}
