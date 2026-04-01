using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionCapacity;

public record UpdateRegistrationSessionCapacityCommand(
    Guid SessionId,
    int MaxSlots) : ICommand<ErrorOr<Success>>;
