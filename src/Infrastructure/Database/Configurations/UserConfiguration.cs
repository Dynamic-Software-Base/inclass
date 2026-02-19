using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value))
            .ValueGeneratedNever();

        builder.Property(user => user.CreatedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(user => user.LastModifiedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(user => user.Email)
            .HasMaxLength(320);

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(32);

        builder.Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.PhoneNumber).IsUnique();

        builder.Ignore(user => user.DomainEvents);
    }
}
