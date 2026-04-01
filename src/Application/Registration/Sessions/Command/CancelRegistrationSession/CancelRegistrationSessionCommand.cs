using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.CancelRegistrationSession;

public record CancelRegistrationSessionCommand(Guid SessionId) : ICommand<ErrorOr<Success>>;
