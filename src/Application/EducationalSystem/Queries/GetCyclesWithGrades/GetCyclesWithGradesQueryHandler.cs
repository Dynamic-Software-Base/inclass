using Application.Abstractions.Interfaces.Repositories;
using Contract.InClass.Response.School.EducationalSystem;
using MediatR;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.EducationalSystem.Queries.GetCyclesWithGrades;

public sealed class GetCyclesWithGradesQueryHandler
    : IRequestHandler<GetCyclesWithGradesQuery, ErrorOr<List<CycleResponse>>>
{
    private readonly IEducationalSystemRepository _repo;

    public GetCyclesWithGradesQueryHandler(IEducationalSystemRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<List<CycleResponse>>> Handle(
        GetCyclesWithGradesQuery request,
        CancellationToken cancellationToken)
    {
        var systemId = EducationalSystemId.From(request.EducationalSystemId);

        bool exists = await _repo.ExistsAsync(systemId, cancellationToken);
        if (!exists)
        {
            return DomainErrors.EducationalSystemErrors.NotFound;
        }


        List<GradeCycleWithGradesDto> cycles = await _repo
            .GetCyclesWithGradesAsync(systemId, cancellationToken);

        return cycles.Select(c => new CycleResponse(
                c.CycleId, c.Code, c.Name_Fr, c.Name_Ar, c.SortOrder,
                c.Grades.Select(g => new GradeResponse(
                        g.Id, g.Code, g.Name_Fr, g.Name_Ar, g.SortOrder))
                    .ToList()))
            .ToList();
    }
}
