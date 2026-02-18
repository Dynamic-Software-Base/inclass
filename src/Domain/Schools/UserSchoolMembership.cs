using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools;

public sealed class UserSchoolMembership : Entity<UserSchoolMembership, UserSchoolMembershipId>
{
    public UserId UserId { get; private set; }
    public SchoolId SchoolId { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private UserSchoolMembership()
    {
    }

    private UserSchoolMembership(
        UserSchoolMembershipId id,
        UserId userId,
        SchoolId schoolId,
        UserRole role,
        bool isActive) : base(id)
    {
        UserId = userId;
        SchoolId = schoolId;
        Role = role;
        IsActive = isActive;
    }

    public static UserSchoolMembership Create(
        UserSchoolMembershipId id,
        UserId userId,
        SchoolId schoolId,
        UserRole role,
        bool isActive = true) =>
        new(id, userId, schoolId, role, isActive);
}
