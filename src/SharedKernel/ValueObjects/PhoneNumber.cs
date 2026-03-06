using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SharedKernel.ValueObjects;

public record PhoneNumber
{
    public string Number { get; set; }

    private PhoneNumber(){}

    private PhoneNumber(string number)
    {
        Number = number;
    }
    public static PhoneNumber Empty() => new PhoneNumber(string.Empty);
    public static ErrorOr<PhoneNumber> Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return DomainErrors.Required(nameof(number));
        }

        string cleanedNumber = Regex.Replace(number.Trim(), @"[\s\-\.\(\)]", "");

        if (!IsValidNumber(cleanedNumber))
        {
            return DomainErrors.PhoneNumberError.InvalidPhoneNumber;
        }
        return new PhoneNumber(cleanedNumber);
    }

    private static bool IsValidNumber(string number)
    {
        // Mobile : 06/07 followed by 8 digits
        // Landline : 05 followed by 8 digits
        if (!number.All(char.IsDigit))
        {
            return false;
        }
        if (!number.StartsWith("06" , StringComparison.CurrentCulture)
            && !number.StartsWith("07", StringComparison.CurrentCulture)
            &&  !number.StartsWith("05", StringComparison.CurrentCulture) )
        {
            return false;
        }
        // 0677446843

        if (number.Length != 10)
        {
            return false;
        }
        return true;
    }

    public override string ToString() => Number;
}
