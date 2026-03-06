using Application.Abstractions.Data;
using Application.Invitations.Common;
using Application.Invitations.Contracts;
using Domain.Invitations;
using Domain.Schools;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;

namespace Application.Invitations.Queries.GetInvitationPreview;

public sealed class GetInvitationPreviewQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInvitationPreviewQuery, ErrorOr<InvitationPreviewResponse>>
{
    public async Task<ErrorOr<InvitationPreviewResponse>> Handle(
        GetInvitationPreviewQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Error.Validation("Invitation.Token.Required", "Invitation token is required.");
        }

        string tokenHash;
        try
        {
            tokenHash = InvitationTokenHasher.ComputeHash(request.Token);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Invitation.Token.Invalid", "Invitation token is invalid.");
        }

        Invitation? invitation = await unitOfWork
            .Set<Invitation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (invitation is null)
        {
            return Error.NotFound("Invitation.NotFound", "Invitation was not found.");
        }

        School? school = await unitOfWork
            .Set<School>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == invitation.SchoolId, cancellationToken);

        if (school is null)
        {
            return Error.NotFound("Invitation.School.NotFound", "School for this invitation was not found.");
        }

        bool userExists = invitation.TargetType switch
        {
            InvitationTargetType.Email => await unitOfWork
                .Set<User>()
                .AsNoTracking()
                .AnyAsync(x => x.Email == invitation.TargetValue, cancellationToken),
            InvitationTargetType.Phone => await unitOfWork
                .Set<User>()
                .AsNoTracking()
                .AnyAsync(x => x.PhoneNumber == invitation.TargetValue, cancellationToken),
            _ => false
        };

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        InvitationStatus effectiveStatus = InvitationStatusEvaluator.GetEffectiveStatus(invitation, utcNow);

        return new InvitationPreviewResponse(
            invitation.SchoolId.Value,
            school.Name,
            invitation.Role,
            invitation.ExpiresAt,
            effectiveStatus,
            InvitationTargetMasker.Mask(invitation.TargetType, invitation.TargetValue),
            userExists,
            effectiveStatus == InvitationStatus.Pending);
    }
}
