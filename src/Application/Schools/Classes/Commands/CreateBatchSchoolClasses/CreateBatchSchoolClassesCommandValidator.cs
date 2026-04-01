using FluentValidation;

namespace Application.Schools.Classes.Commands.CreateBatchSchoolClasses;

public class CreateBatchSchoolClassesCommandValidator
    : AbstractValidator<CreateBatchSchoolClassesCommand>
{
    public CreateBatchSchoolClassesCommandValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty();

        RuleFor(x => x.GradeDefinitionId)
            .NotEmpty();

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{4}$")
            .WithMessage("AcademicYear must follow the format YYYY-YYYY (e.g. 2025-2026).");

        RuleFor(x => x.Classes)
            .NotEmpty()
            .WithMessage("At least one class definition is required.");

        RuleFor(x => x.Classes)
            .Must(classes => classes.Select(c => c.Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)).Distinct().Count() == classes.Count)
            .WithMessage("Class names must be unique within the batch.");

        RuleForEach(x => x.Classes)
            .SetValidator(new SchoolClassDefinitionDtoValidator());
    }
}

public class SchoolClassDefinitionDtoValidator
    : AbstractValidator<SchoolClassDefinitionDto>
{
    public SchoolClassDefinitionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .LessThanOrEqualTo(60);
    }
}
