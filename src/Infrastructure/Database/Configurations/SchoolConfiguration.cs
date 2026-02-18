using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

internal sealed class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("schools");

        builder.HasKey(school => school.Id);

        builder.Property(school => school.Id)
            .HasConversion(
                schoolId => schoolId.Value,
                value => SchoolId.From(value))
            .ValueGeneratedNever();

        builder.Property(school => school.OwnerUserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.CreatedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.LastModifiedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Ignore(school => school.DomainEvents);
    }
}
