using Domain.Invitations;
using SharedKernel.Enums;

namespace Application.Invitations.Common;

internal static class InvitationStatusEvaluator
{
    public static InvitationStatus GetEffectiveStatus(Invitation invitation, DateTimeOffset utcNow)
    {
        if (invitation.Status == InvitationStatus.Pending && invitation.IsExpired(utcNow))
        {
            return InvitationStatus.Expired;
        }

        return invitation.Status;
    }

    public static bool IsValid(Invitation invitation, DateTimeOffset utcNow) =>
        GetEffectiveStatus(invitation, utcNow) == InvitationStatus.Pending;
}
