using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Schools.Contracts;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Schools.Queries.GetMySchools;

public sealed class GetMySchoolsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMySchoolsQuery, ErrorOr<List<MySchoolDto>>>
{
    public async Task<ErrorOr<List<MySchoolDto>>> Handle(
        GetMySchoolsQuery request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return Error.Unauthorized("School.ListMine.Unauthorized", "Authentication is required to list your schools.");
        }

        var membershipRows = await unitOfWork
            .Set<UserSchoolMembership>()
            .AsNoTracking()
            .Where(membership => membership.UserId == currentUser.Id && membership.IsActive)
            .Join(
                unitOfWork
                    .Set<School>()
                    .AsNoTracking(),
                membership => membership.SchoolId,
                school => school.Id,
                (membership, school) => new
                {
                    school.Id,
                    school.Name,
                    membership.Role
                })
            .ToListAsync(cancellationToken);

        var schools = membershipRows
            .GroupBy(row => new { row.Id, row.Name })
            .Select(group => new MySchoolDto(
                group.Key.Id,
                group.Key.Name,
                group.Select(x => x.Role).Distinct().ToList()))
            .OrderBy(x => x.Name)
            .ToList();

        return schools;
    }
}
