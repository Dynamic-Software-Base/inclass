using Application.Invitations.Contracts;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.ResendInvitation;

public sealed record ResendInvitationCommand(
    SchoolId SchoolId,
    InvitationId InvitationId)
    : IRequest<ErrorOr<ResendInvitationResponse>>;
