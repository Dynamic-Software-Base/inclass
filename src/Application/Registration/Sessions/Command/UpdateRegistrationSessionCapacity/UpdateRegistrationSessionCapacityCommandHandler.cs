using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using MediatR;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionCapacity;

public sealed class UpdateRegistrationSessionCapacityCommandHandler
    : IRequestHandler<UpdateRegistrationSessionCapacityCommand, ErrorOr<Success>>
{
    private readonly IRegistrationSessionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateRegistrationSessionCapacityCommandHandler(
        IRegistrationSessionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateRegistrationSessionCapacityCommand request,
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

        ErrorOr<RegistrationCapacity> capacityResult = RegistrationCapacity.Create(request.MaxSlots);
        if (capacityResult.IsError)
        {
            return capacityResult.Errors;
        }

        ErrorOr<Success> result = session.UpdateCapacity(capacityResult.Value, userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
