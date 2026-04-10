using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionPeriod;

public record UpdateRegistrationSessionPeriodCommand(
    Guid SessionId,
    DateOnly  OpenDate,
    DateOnly? CloseDate) : ICommand<ErrorOr<Success>>;
