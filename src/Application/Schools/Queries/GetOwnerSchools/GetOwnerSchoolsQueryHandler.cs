using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Contract.InClass.Response.School;
using MediatR;
using SharedKernel;

namespace Application.Schools.Queries.GetOwnerSchools;

public class GetOwnerSchoolsQueryHandler : IRequestHandler<GetOwnerSchoolsQuery,ErrorOr<List<SchoolSummaryDto>>>
{

    private readonly ISchoolRepository _schoolRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileUrlResolver _fileUrlResolver;
    public GetOwnerSchoolsQueryHandler(ICurrentUserService currentUser, ISchoolRepository schoolRepository, IFileUrlResolver fileUrlResolver)
    {
        _currentUserService = currentUser;
        _schoolRepository = schoolRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<List<SchoolSummaryDto>>> Handle(GetOwnerSchoolsQuery request, CancellationToken cancellationToken)
    {
        ICurrentUser user =  _currentUserService.GetCurrentUser();

        return await _schoolRepository.GetAllOwnerAsync(user.Id,_fileUrlResolver, cancellationToken);

    }
}
