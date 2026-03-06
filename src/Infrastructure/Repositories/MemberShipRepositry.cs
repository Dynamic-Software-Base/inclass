using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class MemberShipRepository :IMemberShipReposiory
{
    private readonly ApplicationDbContext _dbContext;

    public MemberShipRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async  Task<ErrorOr<Success>> AddAsync(UserSchoolMembership membership, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserSchoolMemberships.AddAsync(membership, cancellationToken);

        return Result.Success;
    }
}
