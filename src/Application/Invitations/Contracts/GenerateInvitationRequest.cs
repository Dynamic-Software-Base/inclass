using SharedKernel.Enums;

namespace Application.Invitations.Contracts;

public sealed record GenerateInvitationRequest(
    UserRole Role,
    InvitationTargetType TargetType,
    string TargetValue);
