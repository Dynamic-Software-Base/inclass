using Domain.Registrations;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Infrastructure.Database.Configurations;

public sealed class StudentApplicationConfiguration
    : IEntityTypeConfiguration<StudentApplication>
{
    public void Configure(EntityTypeBuilder<StudentApplication> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("student_applications");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => StudentApplicationId.From(value))
            .IsRequired();

        // ── Foreign Keys ──────────────────────────────────────────────────────
        builder.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .HasConversion(id => id.Value, value => RegistrationSessionId.From(value))
            .IsRequired();

        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id.Value, value => SchoolId.From(value))
            .IsRequired();

        builder.Property(x => x.TargetGradeId)
            .HasColumnName("target_grade_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        builder.Property(x => x.StudentId)
            .HasColumnName("student_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? StudentId.From(value.Value) : (StudentId?)null)
            .IsRequired(false);
        builder.Property(x => x.ParentId)
            .HasColumnName("parent_id")
            .HasConversion(id => id.Value, value => ParentTuteurId.From(value))
            .IsRequired();

        builder.Property(x => x.AssignedClassId)
            .HasColumnName("assigned_class_id")
            .HasConversion(id => id!.Value.Value, value => SchoolClassId.From(value))
            .IsRequired(false);

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

        // ── QueueInfo (optional owned value object) ────────────────────────────
        builder.OwnsOne(x => x.QueueInfo, q =>
        {
            q.Property(x => x.Position)
                .HasColumnName("queue_position")
                .IsRequired();

            q.Property(x => x.ProcessingDate)
                .HasColumnName("queue_processing_date")
                .IsRequired();
        });

        // ── Status ────────────────────────────────────────────────────────────
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // ── ApplicationType ───────────────────────────────────────────────────
        builder.Property(x => x.ApplicationType)
            .HasColumnName("application_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // ── PaymentStatus ─────────────────────────────────────────────────────
        builder.Property(x => x.PaymentStatus)
            .HasColumnName("payment_status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired(false);
// ── ApplicantContact (owned value object) ─────────────────────────────
        builder.OwnsOne(x => x.Contact, c =>
        {
            c.OwnsOne(x => x.Email, e =>
            {
                e.Property(x => x.EmailAddress)
                    .HasColumnName("contact_email")
                    .HasMaxLength(255)
                    .IsRequired(false);
            });

            c.OwnsOne(x => x.PhoneNumber, p =>
            {
                p.Property(x => x.Number)
                    .HasColumnName("contact_phone")
                    .HasMaxLength(20)
                    .IsRequired(false);
            });
        });

        builder.Property(x => x.FormValuesJson)
            .HasColumnName("form_values_json")
            .HasColumnType("jsonb")
            .IsRequired();
        // ── Dates ─────────────────────────────────────────────────────────────
        builder.Property(x => x.SubmittedAt)
            .HasColumnName("submitted_at")
            .IsRequired();

        builder.Property(x => x.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired(false);

        // ── Reschedule flag ───────────────────────────────────────────────────
        builder.Property(x => x.RescheduleUsed)
            .HasColumnName("reschedule_used")
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

        builder.HasIndex(x => new { x.StudentId, x.SessionId })
            .HasDatabaseName("ix_student_applications_student_session")
            .IsUnique();

        builder.HasIndex(x => new { x.SessionId, x.Status })
            .HasDatabaseName("ix_student_applications_session_status");

        builder.HasIndex(x => new { x.SchoolId, x.Status })
            .HasDatabaseName("ix_student_applications_school_status");

        builder.HasIndex(x => x.ParentId)
            .HasDatabaseName("ix_student_applications_parent_id");
    }
}
