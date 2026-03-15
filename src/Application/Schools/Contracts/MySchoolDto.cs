using Domain.Schools;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Contracts;

public sealed record MySchoolDto(
    SchoolId SchoolId,
    string Name,
    List<UserRole> Roles);
