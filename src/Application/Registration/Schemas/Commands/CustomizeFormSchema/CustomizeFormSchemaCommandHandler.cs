using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Schools;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Application.Registration.Schemas.Commands.CustomizeFormSchema;

public class CustomizeFormSchemaCommandHandler : IRequestHandler<CustomizeFormSchemaCommand,ErrorOr<Guid>>
{
    private readonly IRegistrationFormSchemaRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CustomizeFormSchemaCommandHandler(IRegistrationFormSchemaRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CustomizeFormSchemaCommand request, CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);
        var gradeId = GradeDefinitionId.From(request.GradeDefinitionId);
        UserId userId = _currentUser.GetCurrentUser().Id;

        RegistrationFormSchema? existing = await _repository.GetBySchoolAndGradeAsync(schoolId, gradeId, cancellationToken);
        if (existing is not null)
        {
            ErrorOr<Success> updateResult = existing.UpdateSchema(request.FormSchemaJson, userId);
            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            _repository.Update(existing);
            return existing.Id.Value;
        }
        RegistrationFormSchema? defaultSchema = await _repository.GetDefaultByGradeAsync(gradeId,cancellationToken);
        if (defaultSchema is null)
        {

            return Error.NotFound(
                "FormSchema.Default.NotFound",
                "no default schema for this grade level was found");
        }

        ErrorOr<RegistrationFormSchema> forkResult = RegistrationFormSchema.ForkForSchool(
            RegistrationFormSchemaId.New(),
            schoolId,
            gradeId,
            request.FormSchemaJson,
            defaultSchema.BaseVersion,
            userId
        );
        if (forkResult.IsError) { return forkResult.Errors; }
        await _repository.AddAsync(forkResult.Value, cancellationToken);
        return forkResult.Value.Id.Value;
    }
}
