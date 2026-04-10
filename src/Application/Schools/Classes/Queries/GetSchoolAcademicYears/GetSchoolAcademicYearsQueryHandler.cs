// Application/Schools/Classes/Queries/GetSchoolAcademicYears/GetSchoolAcademicYearsQueryHandler.cs

using Application.Abstractions.Data;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Messaging;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Schools.Classes.Queries.GetSchoolAcademicYears;

public sealed class GetSchoolAcademicYearsQueryHandler
    : IRequestHandler<GetSchoolAcademicYearsQuery, ErrorOr<List<string>>>
{
    private readonly IApplicationDbContext _db;

    public GetSchoolAcademicYearsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<List<string>>> Handle(
        GetSchoolAcademicYearsQuery request,
        CancellationToken cancellationToken)
    {
        var schoolId = new SchoolId(request.SchoolId);

        List<string> years = await _db.SchoolClasses
            .Where(c => c.SchoolId == schoolId)
            .Select(c => c.AcademicYear.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Sort descending by start year (e.g. "2025-2026" before "2024-2025")
        return years
            .OrderByDescending(y => y)
            .ToList();
    }
}
