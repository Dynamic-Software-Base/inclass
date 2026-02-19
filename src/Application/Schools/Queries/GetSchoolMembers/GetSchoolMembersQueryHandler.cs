using Application.Abstractions.Data;
using Application.Schools.Contracts;
using Domain.Schools;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Schools.Queries.GetSchoolMembers;

public sealed class GetSchoolMembersQueryHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetSchoolMembersQuery, ErrorOr<List<SchoolMemberDto>>>
{
    public async Task<ErrorOr<List<SchoolMemberDto>>> Handle(
        GetSchoolMembersQuery request,
        CancellationToken cancellationToken)
    {
        var membershipRows = await unitOfWork
            .Set<UserSchoolMembership>()
            .AsNoTracking()
            .Where(membership => membership.SchoolId == request.SchoolId && membership.IsActive)
            .Join(
                unitOfWork
                    .Set<User>()
                    .AsNoTracking()
                    .Where(user => user.IsActive),
                membership => membership.UserId,
                user => user.Id,
                (membership, user) => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    membership.Role
                })
            .ToListAsync(cancellationToken);

        var members = membershipRows
            .GroupBy(row => new
            {
                row.Id,
                row.FullName,
                row.Email,
                row.PhoneNumber
            })
            .Select(group => new SchoolMemberDto(
                group.Key.Id.Value,
                group.Key.FullName,
                group.Key.Email,
                group.Key.PhoneNumber,
                group.Select(x => x.Role).Distinct().ToList()))
            .OrderBy(x => x.FullName)
            .ToList();

        return members;
    }
}
