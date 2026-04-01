using FluentValidation;

namespace Application.Registration.Sessions.Command.CancelRegistrationSession;

public class CancelRegistrationSessionCommandValidator
    : AbstractValidator<CancelRegistrationSessionCommand>
{
    public CancelRegistrationSessionCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
