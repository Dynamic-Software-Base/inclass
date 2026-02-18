using Domain.Invitations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("invitations");

        builder.HasKey(invitation => invitation.Id);

        builder.Property(invitation => invitation.Id)
            .HasConversion(
                invitationId => invitationId.Value,
                value => InvitationId.From(value))
            .ValueGeneratedNever();

        builder.Property(invitation => invitation.SchoolId)
            .HasConversion(
                schoolId => schoolId.Value,
                value => SchoolId.From(value));

        builder.Property(invitation => invitation.CreatedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(invitation => invitation.LastModifiedBy)
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(invitation => invitation.AcceptedByUserId)
            .HasConversion(
                userId => userId.HasValue ? userId.Value.Value : (Guid?)null,
                value => value.HasValue ? UserId.From(value.Value) : null);

        builder.Property(invitation => invitation.TargetValue)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(invitation => invitation.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(invitation => invitation.ExpiresAt)
            .IsRequired();

        builder.Property(invitation => invitation.Status)
            .IsRequired();

        builder.Property(invitation => invitation.Role)
            .IsRequired();

        builder.Property(invitation => invitation.TargetType)
            .IsRequired();

        builder.HasIndex(invitation => invitation.TokenHash)
            .IsUnique();

        builder.HasIndex(invitation => new
        {
            invitation.SchoolId,
            invitation.TargetType,
            invitation.TargetValue,
            invitation.Role,
            invitation.Status
        });

        builder.Ignore(invitation => invitation.DomainEvents);
    }
}
