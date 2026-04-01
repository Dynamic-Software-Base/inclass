using FluentValidation;

namespace Application.Registration.Sessions.Command.CreateRegistratinSession;

public class CreateBatchRegistrationSessionsCommandValidator
    : AbstractValidator<CreateBatchRegistrationSessionsCommand>
{
    public CreateBatchRegistrationSessionsCommandValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty();

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{4}$")
            .WithMessage("AcademicYear must follow the format YYYY-YYYY (e.g. 2025-2026).");

        RuleFor(x => x.GradeDefinitionIds)
            .NotEmpty()
            .WithMessage("At least one grade must be specified.");

        RuleFor(x => x.GradeDefinitionIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate grade definitions are not allowed.");

        RuleFor(x => x.OpenDate)
            .NotEmpty()
            .LessThan(x => x.CloseDate)
            .WithMessage("OpenDate must be before CloseDate.");

        RuleFor(x => x.CloseDate)
            .NotEmpty();

        RuleFor(x => x.AssignmentStrategy)
            .IsInEnum();

        RuleFor(x => x.Phases)
            .NotEmpty()
            .WithMessage("At least one registration phase is required.");

        RuleForEach(x => x.Phases)
            .SetValidator(new RegistrationPhaseRequestValidator());

        RuleFor(x => x.DailyQuota)
            .GreaterThan(0)
            .When(x => x.DailyQuota.HasValue);
    }
}
