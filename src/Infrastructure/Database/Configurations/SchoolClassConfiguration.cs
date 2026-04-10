using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Infrastructure.Database.Configurations;

public sealed class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("school_classes");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => SchoolClassId.From(value))
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

        // ── AcademicYear (owned value object) ─────────────────────────────────
        builder.OwnsOne(x => x.AcademicYear, ay =>
        {
            ay.Property(x => x.Value)
                .HasColumnName("academic_year_value")
                .HasMaxLength(10)
                .IsRequired();

            ay.Property(x => x.StartYear)
                .HasColumnName("academic_year_start_year")
                .IsRequired();

            ay.Property(x => x.EndYear)
                .HasColumnName("academic_year_end_year")
                .IsRequired();
        });
        // ── ClassName (owned value object) ────────────────────────────────────
        builder.OwnsOne(x => x.Name, n =>
        {
            n.Property(x => x.Value)
                .HasColumnName("name")
                .HasMaxLength(20)
                .IsRequired();
        });

        // ── ClassCapacity (owned value object) ────────────────────────────────
        builder.OwnsOne(x => x.Capacity, c =>
        {
            c.Property(x => x.MaxStudents)
                .HasColumnName("capacity_max_students")
                .IsRequired();
        });


        // ── Enrollment counter ────────────────────────────────────────────────
        builder.Property(x => x.CurrentEnrollmentCount)
            .HasColumnName("current_enrollment_count")
            .IsRequired();

        // ── Audit ─────────────────────────────────────────────────────────────
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
        builder.HasIndex(x => new { x.SchoolId, x.GradeDefinitionId })
            .HasDatabaseName("ix_school_classes_school_grade");

        builder.HasIndex(x => x.SchoolId)
            .HasDatabaseName("ix_school_classes_school_id");
    }
}
