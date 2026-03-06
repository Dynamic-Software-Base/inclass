using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace SharedKernel;

public interface ICurrentUser
{
    UserId Id { get; }
    string Email { get; }
    string FullName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<UserRole> Roles { get; }
    bool IsInRole(UserRole role);
    bool IsSchoolOwner => IsInRole(UserRole.SchoolOwner);
    bool IsTeacher => IsInRole(UserRole.Teacher);
    bool IsStudent => IsInRole(UserRole.Student);
    bool IsParent => IsInRole(UserRole.Parent);
    bool IsSchoolAdministrator => IsInRole(UserRole.SchoolAdministrator);
    bool IsPlatformAdmin => IsInRole(UserRole.PlatformAdmin);
}
