using Application.Schools.Contracts;
using Domain.Schools;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Queries.GetSchoolMembers;

public sealed record GetSchoolMembersQuery(SchoolId SchoolId) : IRequest<ErrorOr<List<SchoolMemberDto>>>;
