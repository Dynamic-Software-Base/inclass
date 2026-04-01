using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionPeriod;

public record UpdateRegistrationSessionPeriodCommand(
    Guid SessionId,
    DateTime OpenDate,
    DateTime CloseDate) : ICommand<ErrorOr<Success>>;
