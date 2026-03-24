using Domain.File;
using Domain.Schools;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

internal sealed class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("schools");

        builder.HasKey(school => school.Id);

        builder.Property(school => school.Id)
            .HasColumnName("id")
            .HasConversion(
                schoolId => schoolId.Value,
                value => SchoolId.From(value))
            .ValueGeneratedNever();

        builder.Property(school => school.OwnerUserId)
            .HasColumnName("owner_user_id")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.EducationalSystemId)
            .HasColumnName("educational_system_id")
            .HasConversion(
                id => id.Value,
                value => EducationalSystemId.From(value)
            );
        builder.Property(school => school.CreatedBy)
            .HasColumnName("created_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));
        builder.Property(f => f.LastModifiedAt)
            .HasColumnName("last_modified_at");
        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");
        builder.Property(school => school.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(school => school.Ar_Name)
            .HasColumnName("ar_name")
            .HasMaxLength(200);

        builder.Property(school => school.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        // Address → separate table
        builder.OwnsOne(e => e.Address, addressBuilder =>
        {
            addressBuilder.ToTable("school_addresses");
            addressBuilder.WithOwner().HasForeignKey("school_id");

            addressBuilder.Property(a => a.StreetAddress)
                .HasColumnName("street_address")
                .HasMaxLength(500)
                .IsRequired();

            addressBuilder.Property(a => a.BuildingNumber)
                .HasColumnName("building_number")
                .HasMaxLength(50);

            addressBuilder.Property(a => a.Apartment)
                .HasColumnName("apartment")
                .HasMaxLength(50);

            addressBuilder.Property(a => a.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Province)
                .HasColumnName("province")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Region)
                .HasColumnName("region")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20)
                .IsRequired();

            // Coordinates → nested OwnsOne, stays in school_addresses table
            addressBuilder.OwnsOne(a => a.Coordinates, coordinatesBuilder =>
            {
                coordinatesBuilder.Property(c => c.Latitude)
                    .HasColumnName("latitude");

                coordinatesBuilder.Property(c => c.Longitude)
                    .HasColumnName("longitude");
            });
        });

        // ContactInfo → same table as schools (only 3 columns)
        builder.OwnsOne(s => s.ContactInfo, b =>
        {
            b.Property(c => c.PrimaryPhoneNumber)
                .HasColumnName("primary_phone")
                .HasConversion(
                    phone => phone.Number,
                    value => PhoneNumber.Create(value).Value)
                .HasMaxLength(20)
                .IsRequired();

            b.Property(c => c.SecondaryPhoneNumber)
                .HasColumnName("secondary_phone")
                .HasConversion(
                    phone => phone == null ? null : phone.Number,
                    value => value == null ? null : PhoneNumber.Create(value).Value)
                .HasMaxLength(20);

            b.Property(c => c.Email)
                .HasColumnName("email")
                .HasConversion(
                    email => email.EmailAddress,
                    value => Email.Create(value).Value)
                .HasMaxLength(200)
                .IsRequired();
        });
        builder.OwnsMany(s => s.Pictures, pictureBuilder =>
        {
            pictureBuilder.ToTable("school_pictures");
            pictureBuilder.WithOwner().HasForeignKey("school_id");

            pictureBuilder.Property<int>("id")
                .ValueGeneratedOnAdd();

            pictureBuilder.HasKey("id");
            pictureBuilder.Property(p => p.StoredFileId)
                .HasColumnName("stored_file_id")
                .HasConversion(
                    storedFileId => storedFileId.Value,
                    value => StoredFileId.From(value)
                    )
                .IsRequired();

            pictureBuilder.Property(p => p.AltText)
                .HasColumnName("alt_text")
                .HasMaxLength(500);

            pictureBuilder.Property(p => p.IsMain)
                .HasColumnName("is_main")
                .IsRequired();
                pictureBuilder.HasIndex("school_id", nameof(SchoolPicture.StoredFileId))
                    .IsUnique();
        });
        // GradeLevels → same table as schools (1 column)
        builder.ComplexProperty(s => s.GradeLevels, gradeLevelsBuilder =>
        {
            gradeLevelsBuilder.Property(g => g.Levels)
                .HasColumnName("grade_levels")
                .HasConversion<int>();
        });

        builder.OwnsMany(s => s.SupportedGrades, sg =>
        {
            sg.ToTable("school_supported_grades");
            sg.WithOwner().HasForeignKey(s => s.SchoolId);
            sg.Property(x => x.SchoolId)
                .HasColumnName("school_id")
                .HasConversion(id => id.Value, v => SchoolId.From(v));
            sg.Property(x => x.GradeDefinitionId)
                .HasColumnName("grade_definition_id")
                .HasConversion(id => id.Value, v => new GradeDefinitionId(v));

            sg.Property(x => x.CachedBroadLevel)
                .HasColumnName("cached_broad_level")
                .HasConversion<int>();
            sg.Property(x => x.Capacity)
                .HasColumnName("capacity")
                .HasConversion<int>();
            sg.Property(x => x.IsOffered)
                .HasColumnName("is_offered");
            sg.HasKey(x => new { x.SchoolId, x.GradeDefinitionId });
        });

        builder.Ignore(school => school.DomainEvents);

        builder.HasIndex(s => s.OwnerUserId)
            .HasDatabaseName("ix_schools_owner_user_id");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("ix_schools_name");

    }
}
