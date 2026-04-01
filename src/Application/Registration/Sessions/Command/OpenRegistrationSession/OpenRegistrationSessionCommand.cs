using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.OpenRegistrationSession;

public record OpenRegistrationSessionCommand(Guid SessionId) : ICommand<ErrorOr<Success>>;
