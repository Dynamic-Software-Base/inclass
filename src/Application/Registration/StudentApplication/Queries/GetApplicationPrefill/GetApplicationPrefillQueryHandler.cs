using System.Text.Json;
using Application.Abstractions.Interfaces;
using Contract.InClass.Response.Registration;
using Domain.Registrations.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.StudentApplication.Queries.GetApplicationPrefill;

public sealed class GetApplicationPrefillQueryHandler
    : IRequestHandler<GetApplicationPrefillQuery, ErrorOr<ApplicationPrefillResponse>>
{
    private readonly IApplicationDbContext _db;

    public GetApplicationPrefillQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<ApplicationPrefillResponse>> Handle(
        GetApplicationPrefillQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Find most recent enrolled application by IdentityKey
        var previousApplication = await _db.StudentApplications
            .AsNoTracking()
            .Where(a => a.IdentityKey == request.IdentityKey
                     && a.Status == StudentApplicationStatus.Enrolled)
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new
            {
                a.StudentFirstName,
                a.StudentLastName,
                a.Contact,
                a.FormValuesJson
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (previousApplication is null)
        {
            return Error.NotFound(
                "Prefill.NotFound",
                "Aucun dossier d'inscription finalisé trouvé pour cette clé.");
        }

        // 2. Fetch the target session's grade and resolve current schema
        var sessionId = RegistrationSessionId.From(request.SessionId);

        var session = await _db.RegistrationSessions
            .AsNoTracking()
            .Where(rs => rs.Id == sessionId)
            .Select(rs => new { rs.SchoolId, rs.GradeDefinitionId })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Error.NotFound("Prefill.SessionNotFound",
                "La session d'inscription spécifiée est introuvable.");
        }

        // 3. Resolve schema — school-forked first, global default fallback
        string? schema = await _db.RegistrationFormSchemas
                             .AsNoTracking()
                             .Where(s => s.SchoolId == session.SchoolId
                                         && s.GradeDefinitionId == session.GradeDefinitionId)
                             .Select(s => s.SchemaJson)
                             .FirstOrDefaultAsync(cancellationToken)
                         ??
                         await _db.RegistrationFormSchemas
                             .AsNoTracking()
                             .Where(s => s.SchoolId == null
                                         && s.GradeDefinitionId == session.GradeDefinitionId)
                             .Select(s => s.SchemaJson)
                             .FirstOrDefaultAsync(cancellationToken);

        if (schema is null)
        {
            return Error.NotFound("Prefill.SchemaNotFound",
                "Aucun schéma de formulaire trouvé pour ce niveau.");
        }

        // 4. Extract field keys from current schema (fields sections only, skip info sections)
        List<string> currentSchemaKeys = ExtractFieldKeys(schema);

        // 5. Extract values from previous FormValuesJson that match current schema keys
        Dictionary<string, object?> preFilledValues = IntersectFormValues(
            previousApplication.FormValuesJson,
            currentSchemaKeys);

        return new ApplicationPrefillResponse(
            previousApplication.StudentFirstName,
            previousApplication.StudentLastName,
            previousApplication.Contact.PhoneNumber?.Number ?? string.Empty,
            previousApplication.Contact.Email?.EmailAddress,
            preFilledValues,
            MatchedFieldCount: preFilledValues.Count,
            TotalFieldCount: currentSchemaKeys.Count
        );
    }

    private static List<string> ExtractFieldKeys(string schemaJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(schemaJson);
            return doc.RootElement
                .GetProperty("sections")
                .EnumerateArray()
                .Where(section =>
                    section.TryGetProperty("type", out JsonElement type) &&
                    type.GetString() == "fields")
                .SelectMany(section =>
                    section.GetProperty("fields").EnumerateArray())
                .Select(field =>
                    field.TryGetProperty("key", out JsonElement key) ? key.GetString() : null)
                .Where(key => key is not null)
                .Select(key => key!)
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static Dictionary<string, object?> IntersectFormValues(
        string previousFormValuesJson,
        List<string> currentSchemaKeys)
    {
        var result = new Dictionary<string, object?>();

        try
        {
            using var doc = JsonDocument.Parse(previousFormValuesJson);
            JsonElement root = doc.RootElement;

            foreach (string key in currentSchemaKeys)
            {
                if (root.TryGetProperty(key, out JsonElement value) &&
                    value.ValueKind != JsonValueKind.Null)
                {
                    // deserialize to the most appropriate CLR type
                    result[key] = value.ValueKind switch
                    {
                        JsonValueKind.String  => value.GetString(),
                        JsonValueKind.Number  => value.TryGetInt32(out int i) ? i : value.GetDouble(),
                        JsonValueKind.True    => true,
                        JsonValueKind.False   => false,
                        _                     => value.GetRawText()
                    };
                }
            }
        }
        catch (JsonException)
        {
            // return whatever we managed to collect
        }

        return result;
    }
}
