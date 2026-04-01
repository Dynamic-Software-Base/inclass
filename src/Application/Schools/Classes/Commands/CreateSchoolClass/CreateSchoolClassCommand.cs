using Application.Abstractions.Messaging;

namespace Application.Schools.Classes.Commands.CreateSchoolClass;

public record CreateSchoolClassCommand(
    Guid SchoolId,
    Guid GradeDefinitionId,
    string Name,
    string AcademicYear,
    int MaxStudents
    ): ICommand<ErrorOr<Guid>>;
