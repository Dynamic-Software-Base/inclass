using Domain.Registrations;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class RegistrationSessionConfiguration
    : IEntityTypeConfiguration<RegistrationSession>
{
    public void Configure(EntityTypeBuilder<RegistrationSession> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("registration_sessions");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => RegistrationSessionId.From(value))
            .IsRequired();

        // ── Foreign Keys ──────────────────────────────────────────────────────
        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id.Value, value => SchoolId.From(value))
            .IsRequired();

        builder.Property(x => x.GradeDefinitionId)
            .HasColumnName("grade_definition_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        builder.Property(x => x.FormSchemaId)
            .HasColumnName("form_schema_id")
            .HasConversion(id => id.Value, value => RegistrationFormSchemaId.From(value))
            .IsRequired();

        // ── AcademicYear (owned value object) ────────────────────────────────
        // 3 columns: academic_year_value, academic_year_start_year, academic_year_end_year
        builder.OwnsOne(x => x.AcademicYear, ay =>
        {
            ay.Property(x => x.Value)
                .HasColumnName("academic_year_value")
                .HasMaxLength(10)  // "2025-2026"
                .IsRequired();

            ay.Property(x => x.StartYear)
                .HasColumnName("academic_year_start_year")
                .IsRequired();

            ay.Property(x => x.EndYear)
                .HasColumnName("academic_year_end_year")
                .IsRequired();
        });

        // ── RegistrationPeriod (owned value object) ────────────────────────────
        // 2 columns: period_open_date, period_close_date
        builder.OwnsOne(x => x.Period, p =>
        {
            p.Property(x => x.OpenDate)
                .HasColumnName("period_open_date")
                .IsRequired();

            p.Property(x => x.CloseDate)
                .HasColumnName("period_close_date")
                .IsRequired(false);
        });

        // ── RegistrationCapacity (optional owned value object) ─────────────────
        // 1 column: capacity_max_slots (nullable — null means unlimited)
        builder.OwnsOne(x => x.Capacity, c =>
        {
            c.Property(x => x.MaxSlots)
                .HasColumnName("capacity_max_slots")
                .IsRequired();
        });

        // ── Status ────────────────────────────────────────────────────────────
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()   // stored as enum name string for readability
            .HasMaxLength(50)
            .IsRequired();

        // ── BatchId ───────────────────────────────────────────────────────────
        // Nullable — links sessions created together in a bulk operation
        builder.Property(x => x.BatchId)
            .HasColumnName("batch_id")
            .IsRequired(false);

        // ── Audit (BaseAuditableEntity) ───────────────────────────────────────
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(x => x.LastModifiedAt)
            .HasColumnName("last_modified_at");

        builder.Property(x => x.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(id => id.Value, value => UserId.From(value));

        // ── Indexes ───────────────────────────────────────────────────────────

        // One active session per school+grade per academic year
        builder.HasIndex(x => new { x.SchoolId, x.GradeDefinitionId, x.Status })
            .HasDatabaseName("ix_registration_sessions_school_grade_status");

        builder.HasIndex(x => x.FormSchemaId)
            .HasDatabaseName("ix_registration_sessions_form_schema_id");

        builder.HasIndex(x => x.BatchId)
            .HasDatabaseName("ix_registration_sessions_batch_id");
    }
}
