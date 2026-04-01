using Application.Abstractions.Messaging;
using Domain.Registrations.Enums;

namespace Application.Registration.StudentApplication.Command.SubmitApplication;


public record SubmitApplicationCommand(
    Guid SessionId,
    string StudentFirstName,
    string StudentLastName,
    string ContactPhone,
    string? ContactEmail,
    bool IsReturning,
    string? IdentityKey,
    string FormValuesJson) : ICommand<ErrorOr<Guid>>;
