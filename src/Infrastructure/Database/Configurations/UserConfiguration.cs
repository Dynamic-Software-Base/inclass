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
            .HasColumnName("id")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value))
            .ValueGeneratedNever();

        builder.Property(user => user.CreatedBy)
            .HasColumnName("created_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(user => user.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(user =>  user.LastModifiedAt)
            .HasColumnName("last_modified_at")
            .HasDefaultValueSql("now()");

        builder.Property(user => user.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasMaxLength(320);

        builder.Property(user => user.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(32);

        builder.Property(user => user.FullName)
            .HasColumnName("full_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.PhoneNumber).IsUnique();

        builder.Ignore(user => user.DomainEvents);
    }
}
