using Application.Abstractions.Messaging;

namespace Application.Schools.Classes.Commands.CreateBatchSchoolClasses;

public record CreateBatchSchoolClassesCommand(
    Guid SchoolId,
    Guid GradeDefinitionId,
    string AcademicYear,
    List<SchoolClassDefinitionDto> Classes) : ICommand<ErrorOr<List<Guid>>>;

public record SchoolClassDefinitionDto(
    string Name,
    int MaxStudents);
