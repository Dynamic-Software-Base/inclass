using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Domain.Schools.ValueObjects;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassName;


public sealed class UpdateSchoolClassNameCommandHandler
    : IRequestHandler<UpdateSchoolClassNameCommand, ErrorOr<Success>>
{
    private readonly ISchoolClassRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateSchoolClassNameCommandHandler(
        ISchoolClassRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateSchoolClassNameCommand request,
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

        ErrorOr<ClassName> nameResult = ClassName.Create(request.Name);
        if (nameResult.IsError)
        {
            return nameResult.Errors;
        }

        ErrorOr<Success> result = schoolClass.UpdateName(nameResult.Value, userId);
        if (result.IsError)
        {
            return result.Errors;
        }

        return Result.Success;
    }
}
