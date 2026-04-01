using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.CloseRegistrationSession;

public record CloseRegistrationSessionCommand(Guid SessionId) : ICommand<ErrorOr<Success>>;
