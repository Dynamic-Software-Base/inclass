using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionStrategy;

public record UpdateRegistrationSessionStrategyCommand(
    Guid SessionId,
    int AssignmentStrategy) : ICommand<ErrorOr<Success>>;
