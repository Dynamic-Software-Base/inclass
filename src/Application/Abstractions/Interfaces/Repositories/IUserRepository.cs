using Domain.Users;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
