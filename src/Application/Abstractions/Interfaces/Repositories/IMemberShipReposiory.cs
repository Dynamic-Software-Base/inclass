using Domain.Schools;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IMemberShipReposiory
{
    Task<ErrorOr<Success>> AddAsync(UserSchoolMembership membership, CancellationToken cancellationToken = default);
}
