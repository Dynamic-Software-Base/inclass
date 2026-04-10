using Application.Abstractions.Interfaces;
using Contract.InClass.Response.Registration;
using Domain.Registrations.Enums;
using Domain.Registrations.ValueObjects;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Registration.Sessions.Queries.GetEnrollmentSessionsForSchool;

// Handler
public sealed class GetEnrollmentSessionsForSchoolQueryHandler
    : IRequestHandler<GetEnrollmentSessionsForSchoolQuery, ErrorOr<EnrollmentSessionsResponse>>
{
    private readonly IApplicationDbContext _db;

    public GetEnrollmentSessionsForSchoolQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<EnrollmentSessionsResponse>> Handle(
        GetEnrollmentSessionsForSchoolQuery request,
        CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);

        School? school = await _db.Schools
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == schoolId, cancellationToken);

        if (school is null)
        {
            return Error.NotFound("School.NotFound", "School not found.");
        }

        // single query — join sessions → grade → cycle, filter by school + academic year
        var rows = await (
            from rs in _db.RegistrationSessions.AsNoTracking()
            join g  in _db.GradeDefinitions.AsNoTracking()
                on rs.GradeDefinitionId equals g.Id
            join c  in _db.GradeCycleDefinitions.AsNoTracking()
                on g.GradeCycleDefinitionId equals c.Id
            where rs.SchoolId == schoolId
               && rs.AcademicYear.Value == request.AcademicYear
               && rs.Status != RegistrationSessionStatus.Cancelled
            orderby c.SortOrder, g.SortOrder
            select new
            {
                rs.Id,
                rs.GradeDefinitionId,
                GradeName_Fr  = g.Name_Fr,
                GradeName_Ar  = g.Name_Ar,
                GradeCode     = g.Code,
                CycleId       = c.Id,
                CycleName_Fr  = c.Name_Fr,
                CycleName_Ar  = c.Name_Ar,
                rs.Status,
                rs.Period,
                rs.Capacity,
                rs.ReservedCount,
                rs.EnrolledCount,
                Phases        = rs.Phases
            }
        ).ToListAsync(cancellationToken);

        // resolve active phase in memory (avoids EF translating DateTime.UtcNow comparisons on owned collection)
        DateTime now = DateTime.UtcNow;

        var sessions = rows.Select(r =>
        {
            RegistrationPhase? activePhase = r.Phases
                .FirstOrDefault(p => p.IsActive(now));


            string? activePhaseType = activePhase?.PhaseType switch
            {
                RegistrationPhaseType.ReRegistration => "ReRegistration",
                RegistrationPhaseType.Open           => "Open",
                _                                    => null
            };

            string? activePhaseAllowedType = activePhase?.AllowedApplicantType switch
            {
                AllowedApplicantType.ReturningOnly => "ReturningOnly",
                AllowedApplicantType.All           => "All",
                _                                  => null
            };
            return new EnrollmentSessionDto(
                r.Id.Value,
                r.GradeDefinitionId.Value,
                r.GradeName_Fr,
                r.GradeName_Ar,
                r.GradeCode,
                r.CycleId.Value,
                r.CycleName_Fr,
                r.CycleName_Ar,
                r.Status.ToString(),
                activePhaseType,
                activePhaseAllowedType,
                r.Period.OpenDate.ToDateTime( new TimeOnly(8, 0, 0)),
                r.Period.CloseDate.HasValue ? r.Period.CloseDate.Value.ToDateTime(new TimeOnly(8, 0, 0)) : null,
                r.Capacity.MaxSlots,
                r.ReservedCount,
                r.EnrolledCount
            );
        }).ToList();

        return new EnrollmentSessionsResponse(
            school.Id.Value,
            school.Name,
            request.AcademicYear,
            sessions
        );
    }
}
