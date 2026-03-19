using Domain.Schools;
using ErrorOr;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations;
public sealed class RegistrationFormSchema
    : AggregateRoot<RegistrationFormSchema, RegistrationFormSchemaId>
{

    public SchoolId? SchoolId { get; private set; }

    public GradeDefinitionId GradeDefinitionId { get; private set; }

    public string SchemaJson { get; private set; } = "{}";

    public int Version { get; private set; } = 1;
    public bool IsCustomized { get; private set; }

    public string BaseVersion { get; private set; } = string.Empty;

    private RegistrationFormSchema() { }

    private RegistrationFormSchema(
        RegistrationFormSchemaId id,
        SchoolId? schoolId,
        GradeDefinitionId gradeDefinitionId,
        string schemaJson,
        string baseVersion,
        bool isCustomized,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        SchemaJson = schemaJson;
        BaseVersion = baseVersion;
        IsCustomized = isCustomized;
    }

    public static ErrorOr<RegistrationFormSchema> CreateDefault(
        RegistrationFormSchemaId id,
        GradeDefinitionId gradeDefinitionId,
        string schemaJson,
        string baseVersion,
        UserId createdBy)
    {
        if (string.IsNullOrWhiteSpace(schemaJson))
        {
            return Error.Validation("FormSchema.Json.Empty", "Le contenu du schéma est obligatoire.");
        }


        if (string.IsNullOrWhiteSpace(baseVersion))
        {
            return Error.Validation("FormSchema.BaseVersion.Empty", "La version de base est obligatoire.");
        }


        return new RegistrationFormSchema(
            id, null, gradeDefinitionId, schemaJson, baseVersion, false, createdBy);
    }

    public static ErrorOr<RegistrationFormSchema> ForkForSchool(
        RegistrationFormSchemaId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        string schemaJson,
        string baseVersion,
        UserId createdBy)
    {
        if (string.IsNullOrWhiteSpace(schemaJson))
        {
            return Error.Validation("FormSchema.Json.Empty", "Le contenu du schéma est obligatoire.");
        }


        if (string.IsNullOrWhiteSpace(baseVersion))
        {
            return Error.Validation("FormSchema.BaseVersion.Empty", "La version de base est obligatoire.");
        }


        return new RegistrationFormSchema(
            id, schoolId, gradeDefinitionId, schemaJson, baseVersion, true, createdBy);
    }

    // --- Mutation ---

    public ErrorOr<Success> UpdateSchema(string schemaJson, UserId updatedBy)
    {
        if (string.IsNullOrWhiteSpace(schemaJson))
        {
            return Error.Validation("FormSchema.Json.Empty", "Le contenu du schéma est obligatoire.");
        }


        SchemaJson = schemaJson;
        IsCustomized = SchoolId is not null;
        Version++;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);

        return Result.Success;
    }

    public ErrorOr<Success> ResetToDefault(string defaultSchemaJson, string defaultBaseVersion, UserId updatedBy)
    {
        if (SchoolId is null)
        {
            return Error.Validation("FormSchema.Reset.IsDefault",
                "Le schéma global ne peut pas être réinitialisé.");
        }


        if (string.IsNullOrWhiteSpace(defaultSchemaJson))
        {
            return Error.Validation("FormSchema.Json.Empty", "Le schéma par défaut est vide.");
        }


        SchemaJson = defaultSchemaJson;
        BaseVersion = defaultBaseVersion;
        IsCustomized = false;
        Version++;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);

        return Result.Success;
    }
}
