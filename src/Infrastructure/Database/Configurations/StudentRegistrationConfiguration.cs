using Domain.Registrations;
using Domain.Registrations.Enums;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class StudentRegistrationConfiguration
    : IEntityTypeConfiguration<StudentRegistration>
{
    public void Configure(EntityTypeBuilder<StudentRegistration> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("student_registrations");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => StudentRegistrationId.From(value))
            .IsRequired();

        // ── Foreign Keys ──────────────────────────────────────────────────────
        builder.Property(x => x.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(id => id.Value, value => SchoolId.From(value))
            .IsRequired();

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .HasConversion(id => id.Value, value => RegistrationSessionId.From(value))
            .IsRequired();

        builder.Property(x => x.FormSchemaId)
            .HasColumnName("form_schema_id")
            .HasConversion(id => id.Value, value => RegistrationFormSchemaId.From(value))
            .IsRequired();

        builder.Property(x => x.GradeDefinitionId)
            .HasColumnName("grade_definition_id")
            .HasConversion(id => id.Value, value => GradeDefinitionId.From(value))
            .IsRequired();

        // ── Form Values JSON ──────────────────────────────────────────────────
        // Immutable snapshot of the applicant's answers at submission time.
        // Never modified after submission — treated as an audit record.
        builder.Property(x => x.FormValuesJson)
            .HasColumnName("form_values_json")
            .HasColumnType("jsonb")
            .HasDefaultValue("{}")
            .IsRequired();

        // ── ApplicantContact (owned value object) ─────────────────────────────
        // Flat columns: contact_email, contact_phone
        // Both nullable at DB level — the domain enforces "at least one" invariant
        builder.OwnsOne(x => x.ApplicantContact, contact =>
        {
            contact.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.EmailAddress)
                    .HasColumnName("contact_email")
                    .HasMaxLength(256)
                    .IsRequired(false);
            });

            contact.OwnsOne(c => c.PhoneNumber, phone =>
            {
                phone.Property(p => p.Number)
                    .HasColumnName("contact_phone")
                    .HasMaxLength(20)
                    .IsRequired(false);
            });
        });

        // ── Status ────────────────────────────────────────────────────────────
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(RegistrationStatus.Draft)
            .IsRequired();

        // ── SubmittedAt ───────────────────────────────────────────────────────
        builder.Property(x => x.SubmittedAt)
            .HasColumnName("submitted_at")
            .IsRequired(false);

        // ── ReviewNote (optional owned value object) ──────────────────────────
        // Null until the dossier is approved or rejected.
        // Flat columns: review_reviewed_by, review_reviewed_at, review_comment
        builder.OwnsOne(x => x.ReviewNote, review =>
        {
            review.Property(r => r.ReviewedBy)
                .HasColumnName("review_reviewed_by")
                .HasConversion(id => id.Value, value => UserId.From(value))
                .IsRequired();

            review.Property(r => r.ReviewedAt)
                .HasColumnName("review_reviewed_at")
                .IsRequired();

            review.Property(r => r.Comment)
                .HasColumnName("review_comment")
                .HasMaxLength(1000)
                .IsRequired(false);
        });

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
        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("ix_student_registrations_session_id");

        builder.HasIndex(x => new { x.SchoolId, x.Status })
            .HasDatabaseName("ix_student_registrations_school_status");

        builder.HasIndex(x => x.FormSchemaId)
            .HasDatabaseName("ix_student_registrations_form_schema_id");
    }
}
