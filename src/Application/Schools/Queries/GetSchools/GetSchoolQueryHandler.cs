using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using Contract.InClass.Response.School;
using MediatR;

namespace Application.Schools.Queries.GetSchools;

public sealed class GetSchoolQueryHandler : IRequestHandler<GetSchoolsQuery,ErrorOr<PagedResult<SchoolSummaryDto>>>
{
    private readonly ISchoolRepository _repository;
    private readonly IFileUrlResolver _fileUrlResolver;
    public GetSchoolQueryHandler(ISchoolRepository repository, IFileUrlResolver fileUrlResolver)
    {
        _repository= repository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<PagedResult<SchoolSummaryDto>>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        ErrorOr<PagedResult<SchoolSummaryDto>> result = await _repository.GetPagedAsync(request,_fileUrlResolver, cancellationToken);
        return result;
    }
}
