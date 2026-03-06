using Application.Abstractions.Interfaces.Storage;
using Application.Schools.Queries.GetSchools;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using Domain.Schools;

namespace Application.Abstractions.Interfaces.Repositories;

public interface ISchoolRepository
{
    Task<ErrorOr<Success>> AddAsync(School school, CancellationToken cancellationToken = default);
    Task<ErrorOr<bool>> ExistAsync(string name, CancellationToken cancellationToken = default);

    Task<ErrorOr<PagedResult<SchoolSummaryDto>>> GetPagedAsync(GetSchoolsQuery query,IFileUrlResolver fileResolver, CancellationToken cancellationToken = default);
}
