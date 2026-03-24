using System.Text.Json;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Contract.InClass.Response.Registration.Schemas;
using Domain.Registrations;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Registration.Schemas.Queries.GetFormSchema;

public class GetFormSchemaQueryHandler : IRequestHandler<GetFormSchemaQuery, ErrorOr<FormSchemaResponse>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IApplicationDbContext _context;

    public GetFormSchemaQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<FormSchemaResponse>> Handle(
        GetFormSchemaQuery request,
        CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);
        var gradeId = GradeDefinitionId.From(request.GradeDefinition);

        List<RegistrationFormSchema> schemas = await _context.RegistrationFormSchemas
            .AsNoTracking()
            .Where(s => s.GradeDefinitionId == gradeId
                        && (s.SchoolId == schoolId || s.SchoolId == null))
            .ToListAsync(cancellationToken);

        RegistrationFormSchema? schemaRow = schemas.FirstOrDefault(s => s.SchoolId == schoolId)
                                            ?? schemas.FirstOrDefault(s => s.SchoolId == null);

        if (schemaRow is null)
        {
            return Error.NotFound(
                "FormSchema.NotFound",
                "Aucun schéma trouvé pour ce niveau scolaire.");
        }

        StoredSchemaJson? stored = JsonSerializer.Deserialize<StoredSchemaJson>(
            schemaRow.SchemaJson, JsonOptions);
        if (stored is null)
        {
            return Error.Unexpected(
                "FormSchema.Deserialize.Failed",
                "Le schéma du formulaire est corrompu.");
        }

        return new FormSchemaResponse(
            Id: schemaRow.Id.Value,
            SchemaCode: stored.SchemaCode,
            Title: stored.Title,
            Version: schemaRow.Version,
            IsCustomized: schemaRow.IsCustomized,
            BaseVersion: schemaRow.BaseVersion,
            Sections: stored.Sections);
    }
}

public record StoredSchemaJson(
    string SchemaCode,
    string Title,
    List<FormSectionDto> Sections);
