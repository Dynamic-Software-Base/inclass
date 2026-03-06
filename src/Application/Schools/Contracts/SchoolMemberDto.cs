using SharedKernel.Enums;

namespace Application.Schools.Contracts;

public sealed record SchoolMemberDto(
    Guid UserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    List<UserRole> Roles);
