using Domain.Schools;
using Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Infrastructure.Database.Configurations;

public sealed class ParentTuteurConfiguration
    : IEntityTypeConfiguration<ParentTuteur>
{
    public void Configure(EntityTypeBuilder<ParentTuteur> builder)
    {
        // ── Table ─────────────────────────────────────────────────────────────
        builder.ToTable("parent_tuteurs");

        // ── Primary Key ───────────────────────────────────────────────────────
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => ParentTuteurId.From(value))
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

        // ── MoroccanCin (owned value object) ──────────────────────────────────
        builder.OwnsOne(x => x.CIN, cin =>
        {
            cin.Property(c => c.Value)
                .HasColumnName("cin")
                .HasMaxLength(10)
                .IsRequired();
        });

        // ── PhoneNumber (primary — owned value object) ────────────────────────
        builder.OwnsOne(x => x.PhoneNumber, phone =>
        {
            phone.Property(p => p.Number)
                .HasColumnName("phone_number")
                .HasMaxLength(20)
                .IsRequired();
        });

        // ── SecondaryPhoneNumber (optional owned value object) ────────────────
        builder.OwnsOne(x => x.SecondaryPhoneNumber, phone =>
        {
            phone.Property(p => p.Number)
                .HasColumnName("secondary_phone_number")
                .HasMaxLength(20)
                .IsRequired(false);
        });

        // ── Address (optional owned value object) ─────────────────────────────
        // Coordinates are nested inside Address — double OwnsOne
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.StreetAddress)
                .HasColumnName("address_street")
                .HasMaxLength(250)
                .IsRequired();

            address.Property(a => a.BuildingNumber)
                .HasColumnName("address_building_number")
                .HasMaxLength(10)
                .IsRequired(false);

            address.Property(a => a.Apartment)
                .HasColumnName("address_apartment")
                .HasMaxLength(20)
                .IsRequired(false);

            address.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Province)
                .HasColumnName("address_province")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Region)
                .HasColumnName("address_region")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code")
                .HasMaxLength(5)
                .IsRequired();

            // Coordinates are nested inside Address — another level of OwnsOne
            address.OwnsOne(a => a.Coordinates, coords =>
            {
                coords.Property(c => c.Latitude)
                    .HasColumnName("address_latitude")
                    .HasPrecision(10, 7);

                coords.Property(c => c.Longitude)
                    .HasColumnName("address_longitude")
                    .HasPrecision(10, 7);
            });
        });

        // ── Parental Relation ─────────────────────────────────────────────────
        builder.Property(x => x.Relation)
            .HasColumnName("relation")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // ── IsLegalGuardian ───────────────────────────────────────────────────
        builder.Property(x => x.IsLegalGuardian)
            .HasColumnName("is_legal_guardian")
            .HasDefaultValue(false)
            .IsRequired();

        // ── Email (optional owned value object) ───────────────────────────────
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.EmailAddress)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired(false);
        });

        // ── _studentIds (private collection) ──────────────────────────────────
        // Join table: parent_tuteur_students (parent_tuteur_id, student_id)
        builder.HasMany<Student>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "parent_tuteur_students",
                r => r.HasOne<Student>()
                      .WithMany()
                      .HasForeignKey("student_id")
                      .HasConstraintName("fk_parent_tuteur_students_student_id"),
                l => l.HasOne<ParentTuteur>()
                      .WithMany()
                      .HasForeignKey("parent_tuteur_id")
                      .HasConstraintName("fk_parent_tuteur_students_parent_tuteur_id"),
                j =>
                {
                    j.HasKey("parent_tuteur_id", "student_id");
                    j.ToTable("parent_tuteur_students");
                    j.HasIndex("student_id")
                        .HasDatabaseName("ix_parent_tuteur_students_student_id");
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
            .HasColumnName("last_modified_at");

        builder.Property(x => x.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(id => id.Value, value => UserId.From(value));

        // ── Indexes ───────────────────────────────────────────────────────────
        builder.HasIndex(x => x.RegistrationId)
            .HasDatabaseName("ix_parent_tuteurs_registration_id");

        builder.HasIndex(x => x.SchoolId)
            .HasDatabaseName("ix_parent_tuteurs_school_id");

        // CIN must be unique per school
        builder.HasIndex(x => new { x.SchoolId })
            .HasDatabaseName("ix_parent_tuteurs_school_cin");
    }
}
