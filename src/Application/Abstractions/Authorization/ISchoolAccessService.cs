using Domain.Schools;
using Domain.Users;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Authorization;

public interface ISchoolAccessService
{
    Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        params UserRole[] roles);

    Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        UserRole[] roles,
        CancellationToken cancellationToken);

    Task<List<UserRole>> GetRolesAsync(
        UserId userId,
        SchoolId schoolId,
        CancellationToken cancellationToken = default);
}
