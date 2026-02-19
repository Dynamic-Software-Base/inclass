using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Schools.Contracts;
using Domain.Schools;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Enums;

namespace Application.Schools.Queries.GetSchoolMembers;

public sealed class GetSchoolMembersQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSchoolMembersQuery, ErrorOr<List<SchoolMemberDto>>>
{
    public async Task<ErrorOr<List<SchoolMemberDto>>> Handle(
        GetSchoolMembersQuery request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return Error.Unauthorized("School.Members.Unauthorized", "Authentication is required to list school members.");
        }

        bool canViewMembers = await unitOfWork
            .Set<UserSchoolMembership>()
            .AsNoTracking()
            .AnyAsync(
                membership => membership.UserId == currentUser.Id &&
                              membership.SchoolId == request.SchoolId &&
                              membership.IsActive &&
                              (membership.Role == UserRole.SchoolOwner ||
                               membership.Role == UserRole.SchoolAdministrator),
                cancellationToken);

        if (!canViewMembers)
        {
            return Error.Forbidden(
                "School.Members.Forbidden",
                $"You are not allowed to view members for this school.{currentUser.Id} + {request.SchoolId}");
        }

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
