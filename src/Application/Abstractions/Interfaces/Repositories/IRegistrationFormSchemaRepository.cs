using Domain.Registrations;
using Domain.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IRegistrationFormSchemaRepository
{
    Task<RegistrationFormSchema?> GetBySchoolAndGradeAsync(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        CancellationToken cancellationToken = default
    );

    Task<RegistrationFormSchema?> GetDefaultByGradeAsync(
        GradeDefinitionId gradeDefinitionId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        RegistrationFormSchema schema,
        CancellationToken cancellationToken = default
    );

    void Update(RegistrationFormSchema schema);
}
