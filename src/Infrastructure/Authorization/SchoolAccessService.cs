using Application.Abstractions.Authorization;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authorization;

internal sealed class SchoolAccessService(ApplicationDbContext dbContext) : ISchoolAccessService
{
    public async Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roles);

        if (roles.Count == 0)
        {
            return false;
        }

        return await dbContext.UserSchoolMemberships
            .AsNoTracking()
            .AnyAsync(
                membership => membership.UserId == userId &&
                              membership.SchoolId == schoolId &&
                              membership.IsActive &&
                              roles.Contains(membership.Role),
                cancellationToken);
    }
}
