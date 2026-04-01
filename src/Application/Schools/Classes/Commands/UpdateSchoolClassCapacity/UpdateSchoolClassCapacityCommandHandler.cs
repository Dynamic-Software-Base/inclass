using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Domain.Schools.ValueObjects;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassCapacity;

public sealed class UpdateSchoolClassCapacityCommandHandler
    : IRequestHandler<UpdateSchoolClassCapacityCommand, ErrorOr<Success>>
{
    private readonly ISchoolClassRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateSchoolClassCapacityCommandHandler(
        ISchoolClassRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateSchoolClassCapacityCommand request,
        CancellationToken cancellationToken)
    {
        var classId = SchoolClassId.From(request.SchoolClassId);
        UserId userId = _currentUser.GetCurrentUser().Id;

        SchoolClass? schoolClass = await _repository.GetByIdAsync(classId, cancellationToken);
        if (schoolClass is null)
        {
            return Error.NotFound("SchoolClass.NotFound",
                "The specified class was not found.");
        }

        ErrorOr<ClassCapacity> capacityResult = ClassCapacity.Create(request.MaxStudents);
        if (capacityResult.IsError)
        {
            return capacityResult.Errors;
        }

        ErrorOr<Success> result = schoolClass.UpdateCapacity(capacityResult.Value, userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
