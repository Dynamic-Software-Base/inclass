namespace SharedKernel.ValueObjects.Registration;

public sealed record ApplicantContact
{
    /// <summary>Email de contact — pour notifications d'approbation / rejet</summary>
    public Email? Email { get; }

    /// <summary>Téléphone de contact</summary>
    public PhoneNumber? PhoneNumber { get; }

    private ApplicantContact(Email? email, PhoneNumber? phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static ErrorOr<ApplicantContact> Create(Email? email, PhoneNumber? phoneNumber)
    {
        if (email is null && phoneNumber is null)
        {
            return Error.Validation("ApplicantContact.Empty",
                "Au moins un moyen de contact (email ou téléphone) est obligatoire.");
        }


        return new ApplicantContact(email, phoneNumber);
    }

    public override string ToString() =>
        Email?.EmailAddress ?? PhoneNumber?.Number ?? string.Empty;
}
