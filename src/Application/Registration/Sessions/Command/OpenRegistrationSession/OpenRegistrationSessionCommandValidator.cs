using FluentValidation;

namespace Application.Registration.Sessions.Command.OpenRegistrationSession;

public class OpenRegistrationSessionCommandValidator
    : AbstractValidator<OpenRegistrationSessionCommand>
{
    public OpenRegistrationSessionCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
