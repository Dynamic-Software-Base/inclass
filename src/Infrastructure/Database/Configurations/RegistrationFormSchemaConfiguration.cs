using Domain.Registrations;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class RegistrationFormSchemaConfiguration
    : IEntityTypeConfiguration<RegistrationFormSchema>
{
    public void Configure(EntityTypeBuilder<RegistrationFormSchema> builder)
    {
        // ── Table ────────────────────────────────────────────────────────────
        builder.ToTable("registration_form_schemas");

        // ── Primary Key ──────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => RegistrationFormSchemaId.From(value))
            .IsRequired();

        // ── Foreign Keys ─────────────────────────────────────────────────────

        // SchoolId is nullable — null means this is the default MEN schema
        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id!.Value, value => SchoolId.From(value))
            .IsRequired(false);

        builder.Property(x => x.GradeDefinitionId)
            .HasColumnName("grade_definition_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        // ── Schema JSON ───────────────────────────────────────────────────────
        // Stored as raw jsonb — the application controls serialization.
        // We use jsonb so Postgres can validate and index the JSON if needed.
        builder.Property(x => x.SchemaJson)
            .HasColumnName("schema_json")
            .HasColumnType("jsonb")
            .HasDefaultValue("{}")
            .IsRequired();

        // ── Versioning ────────────────────────────────────────────────────────
        builder.Property(x => x.Version)
            .HasColumnName("version")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(x => x.IsCustomized)
            .HasColumnName("is_customized")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.BaseVersion)
            .HasColumnName("base_version")
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        // ── Audit (BaseAuditableEntity) ───────────────────────────────────────
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(x => x.LastModifiedAt)
            .HasColumnName("last_modified_at")
            .IsRequired(false);

        builder.Property(x => x.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired(false);

        // ── Indexes ───────────────────────────────────────────────────────────

        // Unique schema per school+grade (null school_id = global MEN default)
        builder.HasIndex(x => new { x.SchoolId, x.GradeDefinitionId })
            .HasDatabaseName("ix_registration_form_schemas_school_grade")
            .IsUnique();

        builder.HasIndex(x => x.GradeDefinitionId)
            .HasDatabaseName("ix_registration_form_schemas_grade_definition_id");
    }
}
