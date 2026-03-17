using Application.Abstractions.Authorization;
using Domain.Schools;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authorization;

internal sealed class SchoolAccessService(ApplicationDbContext dbContext) : ISchoolAccessService
{
    public Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        params UserRole[] roles) =>
        HasAnyRoleAsync(userId, schoolId, roles, CancellationToken.None);

    public async Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        UserRole[] roles,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roles);

        if (roles.Length == 0)
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

    public async Task<List<UserRole>> GetRolesAsync(
        UserId userId,
        SchoolId schoolId,
        CancellationToken cancellationToken = default) =>
        await dbContext.UserSchoolMemberships
            .AsNoTracking()
            .Where(membership => membership.UserId == userId &&
                                 membership.SchoolId == schoolId &&
                                 membership.IsActive)
            .Select(membership => membership.Role)
            .Distinct()
            .ToListAsync(cancellationToken);
}
