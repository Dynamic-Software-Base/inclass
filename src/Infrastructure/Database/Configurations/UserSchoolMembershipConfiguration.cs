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
            .HasConversion(
                membershipId => membershipId.Value,
                value => UserSchoolMembershipId.From(value))
            .ValueGeneratedNever();

        builder.Property(membership => membership.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(membership => membership.SchoolId)
            .HasConversion(
                schoolId => schoolId.Value,
                value => SchoolId.From(value));

        builder.Property(membership => membership.Role)
            .IsRequired();

        builder.Property(membership => membership.IsActive)
            .IsRequired();

        builder.HasIndex(membership => new { membership.UserId, membership.SchoolId, membership.Role })
            .IsUnique();
    }
}
