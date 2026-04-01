using FluentValidation;

namespace Application.Schools.Classes.Commands.CreateSchoolClass;

public class CreateSchoolClassCommandValidator : AbstractValidator<CreateSchoolClassCommand>
{
    public CreateSchoolClassCommandValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty();

        RuleFor(x => x.GradeDefinitionId)
            .NotEmpty();

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{4}$")
            .WithMessage("AcademicYear must follow the format YYYY-YYYY (e.g. 2025-2026).");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .LessThanOrEqualTo(60);

    }
}
