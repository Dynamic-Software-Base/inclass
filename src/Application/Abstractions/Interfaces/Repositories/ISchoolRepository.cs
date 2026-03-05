using Domain.Schools;

namespace Application.Abstractions.Interfaces.Repositories;

public interface ISchoolRepository
{
    Task<ErrorOr<Success>> AddAsync(School school, CancellationToken cancellationToken = default);
    Task<ErrorOr<bool>> ExistAsync(string name, CancellationToken cancellationToken = default);
}
