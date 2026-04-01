using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Application.Registration.Sessions.Command.CancelRegistrationSession;
using Domain.Registrations;
using ErrorOr;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Commands.CancelRegistrationSession;

public sealed class CancelRegistrationSessionCommandHandler
    : IRequestHandler<CancelRegistrationSessionCommand, ErrorOr<Success>>
{
    private readonly IRegistrationSessionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CancelRegistrationSessionCommandHandler(
        IRegistrationSessionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        CancelRegistrationSessionCommand request,
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

        ErrorOr<Success> result = session.Cancel(userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
