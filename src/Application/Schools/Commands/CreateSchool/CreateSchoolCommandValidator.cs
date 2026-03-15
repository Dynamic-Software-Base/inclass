using Application.Abstractions.Interfaces.Repositories;
using Domain.File;
using FluentValidation;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{

    private const int MinSchoolNameLength = 5;
    private const int MaxSchoolNameLength = 200;
    private const int MaxSchoolDescription = 2000;

    private const int MaximumPicturesAllowed = 3;

    public CreateSchoolCommandValidator(IStorageFileRepository storageRepo,ISchoolRepository  schoolRepo)
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("School name is required")
            .MinimumLength(MinSchoolNameLength).WithMessage($"School name must be at least {MinSchoolNameLength} characters long")
            .MaximumLength(MaxSchoolNameLength).WithMessage($"School name must be no more than {MaxSchoolNameLength} characters long")
            .WhenAsync(async (command, context, cancellationToken) =>
            {
                ErrorOr<bool> result = await schoolRepo.ExistAsync(command.Name, cancellationToken);
                if (result.IsError)
                {
                    foreach (Error error in result.Errors)
                    {
                        context.AddFailure("SchoolName", error.Description);
                    }

                    return false;
                }
                return true;
            });


        RuleFor(s => s.Ar_Name)
            .MaximumLength(MaxSchoolNameLength)
            .WithMessage($"School name must be no more than {MaxSchoolNameLength} characters long")
            .Matches(@"^[\u0600-\u06FF\s]+$") .WithMessage("Must be in Arabic.")
            .When(s => !string.IsNullOrEmpty(s.Ar_Name));

        RuleFor(s => s.Description)
            .MaximumLength(MaxSchoolDescription).WithMessage($"School description must be no more than {MaxSchoolDescription} characters long")
            .When(s => !string.IsNullOrEmpty(s.Ar_Name));

        RuleFor(s => s.Address)
            .NotNull().WithMessage("Address is required")
            .SetValidator(new CreateSchoolAddressValidator());

        RuleFor(s => s.ContactInfo.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(s =>s.ContactInfo.PrimaryPhoneNumber)
            .NotEmpty().WithMessage("Primary phone number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("Primary phone number is invalid ( must be 10 digits)");

        RuleFor(s => s.ContactInfo.SecondaryPhoneNumber)
            .NotEmpty().WithMessage("Secondary phone number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("Secondary phone number is invalid ( must be 10 digits)")
            .When(s => !string.IsNullOrEmpty(s.ContactInfo.SecondaryPhoneNumber));




        RuleForEach(s => s.Pictures)
            .ChildRules(picture =>
            {
                picture.RuleFor(p => p.PictureId)
                    .NotEmpty().WithMessage("Picture is required")
                    .WhenAsync(async (pic, context,cancellation) =>
                    {
                        ErrorOr<bool> result = await storageRepo.ExistAsync((StoredFileId)pic.PictureId, cancellation);
                        if (result.IsError)
                        {
                            foreach (Error error in result.Errors)
                            {
                                context.AddFailure("PictureId",error.Description);
                            }

                            return false;
                        }

                        if (!result.Value)
                        {
                            context.AddFailure("PictureId","Picture Id Does Not Exist");
                            return false;
                        }

                        return true;
                    });
            });

        RuleFor(s => s.Pictures)
            .NotNull()
            .NotEmpty().WithMessage("At least One Picture required")
            .Must(list => list.Count <= MaximumPicturesAllowed).WithMessage("Cannot have more then 3 pictures")
            .Must(list => list.Count(p => p.IsMain) == 1 ).WithMessage("Only one Main Picture Allowed");

    }
}

public class CreateSchoolAddressValidator : AbstractValidator<CreateSchoolAddress>
{

    private const int MaximumCityLength = 200;
    private const int MaximumStreetLength = 200;
    private const int PostalCodeLength = 20;
    public CreateSchoolAddressValidator()
    {
        RuleFor(x => x.StreetAddress)
            .NotEmpty()
            .WithMessage("Street is required when providing an address.")
            .MaximumLength(MaximumStreetLength);
        RuleFor(x => x.Province)
            .NotEmpty()
            .WithMessage("Province is required when providing an address.")
            .MaximumLength(MaximumStreetLength);
        RuleFor(x => x.Region)
            .NotEmpty()
            .WithMessage("Region is required when providing an address.")
            .MaximumLength(MaximumStreetLength);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required when providing an address.")
            .MaximumLength(MaximumCityLength);


        RuleFor(x => x.PostalCode)
            .Must(pc => pc.Length == PostalCodeLength).WithMessage($"PostalCode must be {PostalCodeLength} characters long");



    }
}
