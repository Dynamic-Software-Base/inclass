using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.RevokeInvitation;

public sealed record RevokeInvitationCommand(
    SchoolId SchoolId,
    InvitationId InvitationId)
    : IRequest<ErrorOr<Success>>;
