using SharedKernel.Enums;

namespace Application.Invitations.Contracts;

public sealed record InvitationPreviewResponse(
    Guid SchoolId,
    string SchoolName,
    UserRole Role,
    DateTimeOffset ExpiresAt,
    InvitationStatus Status,
    string TargetMasked,
    bool UserExists,
    bool IsValid);
