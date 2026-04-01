using FluentValidation;

namespace Application.Registration.Sessions.Command.CloseRegistrationSession;

public class CloseRegistrationSessionCommandValidator
    : AbstractValidator<CloseRegistrationSessionCommand>
{
    public CloseRegistrationSessionCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
