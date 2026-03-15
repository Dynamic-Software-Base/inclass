using Domain.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Contracts;

public sealed record CreateSchoolResponse(SchoolId SchoolId, string Name);
