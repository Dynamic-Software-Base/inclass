using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Registrations.ValueObjects;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionQuota;

public sealed class UpdateRegistrationSessionQuotaCommandHandler
    : IRequestHandler<UpdateRegistrationSessionQuotaCommand, ErrorOr<Success>>
{
    private readonly IRegistrationSessionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateRegistrationSessionQuotaCommandHandler(
        IRegistrationSessionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateRegistrationSessionQuotaCommand request,
        CancellationToken cancellationToken)
    {
        var sessionId = RegistrationSessionId.From(request.SessionId);
        UserId userId = _currentUser.GetCurrentUser().Id;

        RegistrationSession? session = await _repository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return Error.NotFound("RegistrationSession.NotFound",
                "The specified registration session was not found.");
        }

        ErrorOr<DailyProcessingQuota> quotaResult =
            DailyProcessingQuota.Create(request.DailyQuota);
        if (quotaResult.IsError)
        {
            return quotaResult.Errors;
        }

        ErrorOr<Success> result = session.UpdateQuota(
            quotaResult.Value,
            request.DailyCutoffTime,
            userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
