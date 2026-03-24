using Domain.Schools;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

internal sealed class UserSchoolMembershipConfiguration : IEntityTypeConfiguration<UserSchoolMembership>
{
    public void Configure(EntityTypeBuilder<UserSchoolMembership> builder)
    {
        builder.ToTable("user_school_memberships");

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.Id)
            .HasColumnName("id")
            .HasConversion(
                membershipId => membershipId.Value,
                value => UserSchoolMembershipId.From(value))
            .ValueGeneratedNever();

        builder.Property(membership => membership.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(membership => membership.SchoolId)
            .HasColumnName("school_id")
            .HasConversion(
                schoolId => schoolId.Value,
                value => SchoolId.From(value));

        builder.Property(membership => membership.Role)
            .HasColumnName("role")
            .IsRequired();

        builder.Property(membership => membership.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(membership => new { membership.UserId, membership.SchoolId, membership.Role })
            .IsUnique();
    }
}
