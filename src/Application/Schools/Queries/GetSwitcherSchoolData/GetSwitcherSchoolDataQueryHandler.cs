using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Contract.InClass.Response.School;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Queries.GetSwitcherSchoolData;

public class GetSwitcherSchoolDataQueryHandler : IRequestHandler<GetSwitcherSchoolDataQuery,ErrorOr<List<SchoolSwitcherDto>>>
{
    private readonly ICurrentUserService currentUser;
    private readonly IApplicationDbContext dbContext;

    public GetSwitcherSchoolDataQueryHandler(ICurrentUserService currentUser, IApplicationDbContext dbContext)
    {
        this.currentUser = currentUser;
        this.dbContext = dbContext;
    }

    public async Task<ErrorOr<List<SchoolSwitcherDto>>> Handle(GetSwitcherSchoolDataQuery request, CancellationToken cancellationToken)
    {
        UserId userId = currentUser.GetCurrentUser().Id;
        return await dbContext.Schools.AsNoTracking()
            .Where(s => s.OwnerUserId == userId.Value)
            .Select(s => new SchoolSwitcherDto(
                s.Id.Value,
                s.Name,
                s.Address.City))
            .ToListAsync(cancellationToken);
    }
}
