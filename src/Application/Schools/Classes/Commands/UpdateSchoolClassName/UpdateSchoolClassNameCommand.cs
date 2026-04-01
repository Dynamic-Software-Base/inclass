using Application.Abstractions.Messaging;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassName;

public record UpdateSchoolClassNameCommand(
    Guid SchoolClassId,
    string Name) : ICommand<ErrorOr<Success>>;
