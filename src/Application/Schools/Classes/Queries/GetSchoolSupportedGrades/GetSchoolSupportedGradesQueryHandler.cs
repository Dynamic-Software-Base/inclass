using Application.Abstractions.Interfaces;
using Contract.InClass.Response.School.Classes;
using Domain.EducationalSystem.Entities;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Classes.Queries.GetSchoolSupportedGrades;

public sealed class GetSchoolSupportedGradesQueryHandler :
    IRequestHandler<GetSchoolSupportedGradesQuery,ErrorOr<SchoolSupportedGradesResponse>>
{
    private readonly IApplicationDbContext _db;

    public GetSchoolSupportedGradesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async  Task<ErrorOr<SchoolSupportedGradesResponse>> Handle(GetSchoolSupportedGradesQuery request, CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);

        List<GradeDefinitionId> supportedGradeIds = await _db.Schools
            .Where(s => s.Id == schoolId)
            .SelectMany(s => s.SupportedGrades)
            .Where(sg => sg.IsOffered)
            .Select(sg => sg.GradeDefinitionId)
            .ToListAsync(cancellationToken);

        if (!supportedGradeIds.Any())
        {
            return new SchoolSupportedGradesResponse(new List<GradeCycleDto>());
        }

        List<GradeDefinition> grades = await _db.GradeDefinitions
            .Include(g => g.Cycle)
            .Where(g => supportedGradeIds.Contains(g.Id) && g.IsActive)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);

        var cycles = grades
            .GroupBy(g => g.Cycle)
            .Select(grp => new GradeCycleDto(
                Code:       grp.Key.Code,
                NameFr:     grp.Key.Name_Fr,
                NameAr:     grp.Key.Name_Ar,
                BroadLevel: grp.Key.BroadLevel.ToString(),
                SortOrder:  grp.Key.SortOrder,
                Grades: grp.Select(g => new GradeDefinitionDto(
                    Id:        g.Id.Value,
                    Code:      g.Code,
                    NameFr:    g.Name_Fr,
                    NameAr:    g.Name_Ar,
                    SortOrder: g.SortOrder
                )).ToList()
            ))
            .OrderBy(c => c.SortOrder)
            .ToList();

        return new SchoolSupportedGradesResponse(cycles);
    }
}
