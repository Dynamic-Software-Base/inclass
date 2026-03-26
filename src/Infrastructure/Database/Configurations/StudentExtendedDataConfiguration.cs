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
        builder.ToTable("student_extended_data");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => StudentExtendedDataId.From(value))
            .IsRequired();

        builder.Property(x => x.StudentId)
            .HasColumnName("student_id")
            .HasConversion(id => id.Value, value => StudentId.From(value))
            .IsRequired();

        builder.Property(x => x.FieldKey)
            .HasColumnName("field_key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FieldValue)
            .HasColumnName("field_value")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.FieldType)
            .HasColumnName("field_type")
            .HasMaxLength(50)
            .IsRequired();

        // One row per student per field key
        builder.HasIndex(x => new { x.StudentId, x.FieldKey })
            .HasDatabaseName("ix_student_extended_data_student_field")
            .IsUnique();

        builder.HasIndex(x => new { x.FieldKey, x.FieldValue })
            .HasDatabaseName("ix_student_extended_data_field_value");
    }
}
