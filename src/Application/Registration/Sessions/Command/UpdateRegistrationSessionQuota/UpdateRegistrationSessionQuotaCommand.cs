using Application.Abstractions.Messaging;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionQuota;

public record UpdateRegistrationSessionQuotaCommand(
    Guid SessionId,
    int DailyQuota,
    TimeOnly? DailyCutoffTime = null) : ICommand<ErrorOr<Success>>;
