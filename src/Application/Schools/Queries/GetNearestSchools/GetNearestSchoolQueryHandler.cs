using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Contract.InClass.Response.School;
using MediatR;

namespace Application.Schools.Queries.GetNearestSchools;

public sealed  class GetNearestSchoolQueryHandler : IRequestHandler<GetNearestSchoolsQuery,ErrorOr<List<NearestSchoolDto>>>
{
    private readonly ISchoolRepository _repository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetNearestSchoolQueryHandler(
        ISchoolRepository repository,
        IFileUrlResolver fileUrlResolver)
    {
        _repository = repository;
        _fileUrlResolver = fileUrlResolver;
    }
    public async  Task<ErrorOr<List<NearestSchoolDto>>> Handle(GetNearestSchoolsQuery request, CancellationToken cancellationToken)
    {
        if (request.Count is < 1 or > 20)
        {
            return Error.Validation(
                "GetNearestSchools.InvalidCount",
                "Count must be between 1 and 20.");
        }
        List<NearestSchoolDto> result = await _repository.GetNearestSchoolsAsync(
            request.Latitude,
            request.Longitude,
            request.Count,
            _fileUrlResolver,
            cancellationToken);

        return result;
    }
}
