using FluentValidation;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionQuota;

public class UpdateRegistrationSessionQuotaCommandValidator
    : AbstractValidator<UpdateRegistrationSessionQuotaCommand>
{
    public UpdateRegistrationSessionQuotaCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.DailyQuota).GreaterThan(0);
    }
}
