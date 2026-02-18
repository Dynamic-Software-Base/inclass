using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Authorization;

public interface ISchoolAccessService
{
    Task<bool> HasAnyRoleAsync(
        UserId userId,
        SchoolId schoolId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken = default);
}
