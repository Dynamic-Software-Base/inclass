using Domain.Schools;
using Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class StudentConfiguration
    : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("students");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => StudentId.From(value))
            .IsRequired();

        // ── Foreign Keys ──────────────────────────────────────────────────────
        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id.Value, value => SchoolId.From(value))
            .IsRequired();

        builder.Property(x => x.RegistrationId)
            .HasColumnName("registration_id")
            .HasConversion(id => id.Value, value => StudentRegistrationId.From(value))
            .IsRequired();

        builder.Property(x => x.GradeDefinitionId)
            .HasColumnName("grade_definition_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        // ── FullName (owned value object) ─────────────────────────────────────
        builder.OwnsOne(x => x.FullName, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100)
                .IsRequired();
        });

        // ── DateOfBirth ───────────────────────────────────────────────────────
        builder.Property(x => x.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        // ── Gender ────────────────────────────────────────────────────────────
        builder.Property(x => x.Gender)
            .HasColumnName("gender")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // ── NationalId (optional owned value object) ──────────────────────────
        // Two columns: national_id_value + national_id_type
        // Null = not yet assigned at registration time
        builder.OwnsOne(x => x.NationalId, nid =>
        {
            nid.Property(n => n.Value)
                .HasColumnName("national_id_value")
                .HasMaxLength(20)
                .IsRequired();

            nid.Property(n => n.Type)
                .HasColumnName("national_id_type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });

        // ── AcademicYear (owned value object) ─────────────────────────────────
        builder.OwnsOne(x => x.AcademicYear, ay =>
        {
            ay.Property(a => a.Value)
                .HasColumnName("academic_year_value")
                .HasMaxLength(10)
                .IsRequired();

            ay.Property(a => a.StartYear)
                .HasColumnName("academic_year_start_year")
                .IsRequired();

            ay.Property(a => a.EndYear)
                .HasColumnName("academic_year_end_year")
                .IsRequired();
        });

        // ── IsActive ──────────────────────────────────────────────────────────
        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
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
        builder.HasIndex(x => x.RegistrationId)
            .HasDatabaseName("ix_students_registration_id");

        builder.HasIndex(x => new { x.SchoolId, x.GradeDefinitionId, x.IsActive })
            .HasDatabaseName("ix_students_school_grade_active");
    }
}
