namespace SharedKernel.ValueObjects;

public record Email
{
    public string EmailAddress { get; init; }

    private Email (){}

    private Email(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public static ErrorOr<Email> Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return DomainErrors.Required(nameof(emailAddress));
        }

        if (!IsValidEmail(emailAddress))
        {
            return DomainErrors.EmailError.InvalidEmailAddress;
        }

        return new Email(emailAddress);
    }

    private static bool IsValidEmail(string emailAddress)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(emailAddress);
            return addr.Address == emailAddress;
        }
        catch
        {
           return false;
        }
    }
}
