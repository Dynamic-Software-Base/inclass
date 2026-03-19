using Domain.Schools;
using Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class StudentExtendedDataConfiguration
    : IEntityTypeConfiguration<StudentExtendedData>
{
    public void Configure(EntityTypeBuilder<StudentExtendedData> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("student_extended_data");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => StudentExtendedDataId.From(value))
            .IsRequired();

        // ── Foreign Keys ──────────────────────────────────────────────────────
        builder.Property(x => x.StudentId)
            .HasColumnName("student_id")
            .HasConversion(id => id.Value, value => StudentId.From(value))
            .IsRequired();

        // Denormalized for composite index — avoids join with Student on every filter query
        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id.Value, value => SchoolId.From(value))
            .IsRequired();

        builder.Property(x => x.GradeDefinitionId)
            .HasColumnName("grade_definition_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        // ── Field Data ────────────────────────────────────────────────────────
        // FieldKey corresponds to FormField.Key in the JSON schema
        // e.g. "blood_type", "previous_school_name", "has_learning_disabilities"
        builder.Property(x => x.FieldKey)
            .HasColumnName("field_key")
            .HasMaxLength(100)
            .IsRequired();

        // All values serialized as string regardless of original type
        // Bool → "true"/"false", Date → "2008-03-15", Select → "O+"
        builder.Property(x => x.FieldValue)
            .HasColumnName("field_value")
            .HasMaxLength(1000)
            .IsRequired(false);

        // Copied from schema at promotion time — used by UI to render correct filter input
        // e.g. "select", "boolean", "date", "text"
        builder.Property(x => x.FieldType)
            .HasColumnName("field_type")
            .HasMaxLength(50)
            .IsRequired();

        // ── Indexes ───────────────────────────────────────────────────────────

        // Primary access pattern: "all extended data for student X"
        builder.HasIndex(x => x.StudentId)
            .HasDatabaseName("ix_student_extended_data_student_id");

        // Filter pattern: "all students in school X grade Y with field_key = blood_type"
        builder.HasIndex(x => new { x.SchoolId, x.GradeDefinitionId, x.FieldKey })
            .HasDatabaseName("ix_student_extended_data_school_grade_field_key");

        // One value per field per student — no duplicate field keys for the same student
        builder.HasIndex(x => new { x.StudentId, x.FieldKey })
            .HasDatabaseName("ix_student_extended_data_student_field_key_unique")
            .IsUnique();
    }
}
