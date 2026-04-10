using Application.Abstractions.Interfaces;
using Contract.InClass.Response.Registration.Sessions;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Registration.Sessions.Queries.GetRegistrationSessions;

public sealed class GetRegistrationSessionsQueryHandler
    : IRequestHandler<GetRegistrationSessionsQuery, ErrorOr<List<RegistrationSessionResponse>>>
{
    private readonly IApplicationDbContext _db;

    public GetRegistrationSessionsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<List<RegistrationSessionResponse>>> Handle(
        GetRegistrationSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var schoolId = new SchoolId(request.SchoolId);
        var raw = await _db.RegistrationSessions
            .Where(s => s.SchoolId == schoolId
                        && s.AcademicYear.Value == request.AcademicYear)
            .Join(_db.GradeDefinitions.Include(g => g.Cycle),
                s => s.GradeDefinitionId,
                g => g.Id,
                (s, g) => new { s, g })
            .ToListAsync(cancellationToken);
        var sessions = raw
            .Select(x => new RegistrationSessionResponse(
                x.s.Id.Value,
                x.g.Id.Value,
                x.g.Name_Fr,
                x.g.Cycle.Name_Fr,
                x.s.AcademicYear.Value,
                (int)x.s.Status,                        // ✅ in-memory cast
                x.s.Period.OpenDate.ToDateTime(new TimeOnly(8,00,0)),
                x.s.Period.CloseDate.HasValue ? x.s.Period.CloseDate.Value.ToDateTime(new TimeOnly(8, 0, 0)) : null,
                x.s.Capacity.MaxSlots,
                x.s.ReservedCount,
                x.s.EnrolledCount,
                x.s.WaitlistCount,
                x.s.ProcessingQuota?.Value,
#pragma warning disable CA1305
                x.s.DailyCutoffTime.ToString("HH:mm"),
#pragma warning restore CA1305
                (int)x.s.AssignmentStrategy,            // ✅ in-memory cast
                x.s.Phases.Select(p => new RegistrationPhaseResponse(
                    p.StartDate,
                    p.EndDate,
                    (int)p.PhaseType,                   // ✅ in-memory cast
                    (int)p.AllowedApplicantType)).ToList()))
            .ToList();

        return sessions;
    }
}
