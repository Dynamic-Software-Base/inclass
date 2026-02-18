using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommand(string Token) : IRequest<ErrorOr<Success>>;
