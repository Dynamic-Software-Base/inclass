using Application.Abstractions.Interfaces.Repositories;
using Contract.InClass.Response.School.EducationalSystem;
using MediatR;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.EducationalSystem.Queries.GetDefaultEducationalSystem;

public class GetDefaultEducationalSystemQueryHandler: IRequestHandler<GetDefaultEducationalSystemQuery, ErrorOr<EducationalSystemResponse>>
{

    private static readonly EducationalSystemId MenSystemId =
        EducationalSystemId.From(new Guid("10000000-0000-0000-0000-000000000001"));

    private readonly IEducationalSystemRepository _repo;

    public GetDefaultEducationalSystemQueryHandler(IEducationalSystemRepository repo)
    {
        _repo = repo;
    }
    public async Task<ErrorOr<EducationalSystemResponse>> Handle(
        GetDefaultEducationalSystemQuery request,
        CancellationToken cancellationToken)
    {
        bool exists = await _repo.ExistsAsync(MenSystemId, cancellationToken);
        if (!exists)
        {
            return DomainErrors.EducationalSystemErrors.NotFound;
        }


        List<GradeCycleWithGradesDto> cycles = await _repo.GetCyclesWithGradesAsync(
            MenSystemId, cancellationToken);

        return new EducationalSystemResponse(
            MenSystemId.Value,
            "MEN_MA",
            "Système national marocain (MEN)",
            "المنظومة التربوية الوطنية المغربية",
            cycles.Select(c => new CycleResponse(
                    c.CycleId,
                    c.Code,
                    c.Name_Fr,
                    c.Name_Ar,
                    c.SortOrder,
                    c.Grades.Select(g => new GradeResponse(
                            g.Id, g.Code, g.Name_Fr, g.Name_Ar, g.SortOrder))
                        .ToList()))
                .ToList());
    }
}
