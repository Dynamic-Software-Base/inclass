using SharedKernel.Enums;

namespace Application.Schools.Contracts;

public sealed record MySchoolDto(
    Guid SchoolId,
    string Name,
    List<UserRole> Roles);
