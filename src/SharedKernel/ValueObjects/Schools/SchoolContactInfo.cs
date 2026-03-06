namespace SharedKernel.ValueObjects.Schools;

public record SchoolContactInfo
{
    public PhoneNumber PrimaryPhoneNumber { get; init; }
    public PhoneNumber? SecondaryPhoneNumber { get; init; }

    public Email Email { get; init; }

    private SchoolContactInfo() { }

    private SchoolContactInfo(PhoneNumber primaryPhoneNumber, PhoneNumber secondaryPhoneNumber , Email email)
    {
        PrimaryPhoneNumber = primaryPhoneNumber;
        SecondaryPhoneNumber = secondaryPhoneNumber;
        Email = email;
    }
    public static ErrorOr<SchoolContactInfo> Create(
        string primaryPhoneNumber,
        string? secondaryPhone,
        string email)
    {
        var errors = new List<Error>();

        ErrorOr<PhoneNumber> primaryPhoneResult = PhoneNumber.Create(primaryPhoneNumber);
        if (primaryPhoneResult.IsError)
        {
            errors.AddRange(primaryPhoneResult.Errors);
        }

        var secondaryPhoneNumber = PhoneNumber.Empty();
        if (!string.IsNullOrWhiteSpace(secondaryPhone))
        {
            ErrorOr<PhoneNumber> secondaryResult = PhoneNumber.Create(secondaryPhone);
            if (secondaryResult.IsError)
            {
                errors.AddRange(secondaryResult.Errors);
            }
            else
            {
                secondaryPhoneNumber = secondaryResult.Value;
            }
        }

        ErrorOr<Email> emailResult = Email.Create(email);
        if (emailResult.IsError)
        {
            errors.AddRange(emailResult.Errors);
        }

        if (errors.Count > 0)
        {
            return errors;
        }
        return new SchoolContactInfo(primaryPhoneResult.Value, secondaryPhoneNumber,  emailResult.Value);
    }
}
