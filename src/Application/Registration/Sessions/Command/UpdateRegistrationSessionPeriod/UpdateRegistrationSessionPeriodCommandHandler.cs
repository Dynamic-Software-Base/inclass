using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using MediatR;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionPeriod;


public sealed class UpdateRegistrationSessionPeriodCommandHandler
    : IRequestHandler<UpdateRegistrationSessionPeriodCommand, ErrorOr<Success>>
{
    private readonly IRegistrationSessionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateRegistrationSessionPeriodCommandHandler(
        IRegistrationSessionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateRegistrationSessionPeriodCommand request,
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

        ErrorOr<RegistrationPeriod> periodResult =
            RegistrationPeriod.Create(request.OpenDate, request.CloseDate);
        if (periodResult.IsError)
        {
            return periodResult.Errors;
        }

        ErrorOr<Success> result = session.UpdatePeriod(periodResult.Value, userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
