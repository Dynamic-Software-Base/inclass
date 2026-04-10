using Application.Abstractions.Interfaces;
using Contract.InClass.Response.Registration;
using Domain.Registrations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Queries.GetFormSchemaForSession;

public sealed class GetFormSchemaForSessionQueryHandler
    : IRequestHandler<GetFormSchemaForSessionQuery, ErrorOr<FormSchemaResponse>>
{
    private readonly IApplicationDbContext _db;

    public GetFormSchemaForSessionQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<FormSchemaResponse>> Handle(
        GetFormSchemaForSessionQuery request,
        CancellationToken cancellationToken)
    {
        var sessionId = RegistrationSessionId.From(request.SessionId);

        // fetch only what we need — no tracking, no full aggregate load
        var session = await _db.RegistrationSessions
            .AsNoTracking()
            .Where(rs => rs.Id == sessionId)
            .Select(rs => new { rs.SchoolId, rs.GradeDefinitionId })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Error.NotFound("Session.NotFound", "Registration session not found.");
        }

        // school-forked schema takes priority, fall back to global default
        RegistrationFormSchema? schema = await _db.RegistrationFormSchemas
                                             .AsNoTracking()
                                             .Where(s => s.SchoolId == session.SchoolId
                                                         && s.GradeDefinitionId == session.GradeDefinitionId)
                                             .FirstOrDefaultAsync(cancellationToken)
                                         ??
                                         await _db.RegistrationFormSchemas
                                             .AsNoTracking()
                                             .Where(s => s.SchoolId == null
                                                         && s.GradeDefinitionId == session.GradeDefinitionId)
                                             .FirstOrDefaultAsync(cancellationToken);

        if (schema is null)
        {
            return Error.NotFound("Schema.NotFound", "No form schema found for this grade.");
        }

        return new FormSchemaResponse(
            schema.Id.Value,
            schema.GradeDefinitionId.Value,
            schema.IsCustomized,
            schema.SchemaJson,
            schema.Version
        );
    }
}
