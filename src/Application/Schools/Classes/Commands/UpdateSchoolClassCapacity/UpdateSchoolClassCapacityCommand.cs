using Application.Abstractions.Messaging;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassCapacity;

public record UpdateSchoolClassCapacityCommand(
    Guid SchoolClassId,
    int MaxStudents) : ICommand<ErrorOr<Success>>;
