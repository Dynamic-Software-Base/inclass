using Application.Abstractions.Interfaces;
using Contract.InClass.Response.School.Classes;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Schools.Classes.Queries.GetSchoolClasses;


public sealed class GetSchoolClassesQueryHandler
    : IRequestHandler<GetSchoolClassesQuery, ErrorOr<List<SchoolClassResponse>>>
{
    private readonly IApplicationDbContext _db;

    public GetSchoolClassesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<List<SchoolClassResponse>>> Handle(GetSchoolClassesQuery request, CancellationToken cancellationToken)
    {
        var schoolId = new SchoolId(request.SchoolId);

        List<SchoolClassResponse> classes = await _db.SchoolClasses
            .Where(c => c.SchoolId == schoolId
                        && c.AcademicYear.Value == request.AcademicYear)
            .Select(c => new SchoolClassResponse(
                c.Id.Value,
                c.GradeDefinitionId.Value,
                c.AcademicYear.Value,
                c.Name.Value,
                c.Capacity.MaxStudents,
                c.CurrentEnrollmentCount))
            .ToListAsync(cancellationToken);

        return classes;
    }
}
