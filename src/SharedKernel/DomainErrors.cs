namespace SharedKernel;

public static class DomainErrors
{
    public static Error Required(string propertyName) => Error.Validation("property.validation",$"{propertyName} is required");
    public static class AddressError
    {
        public static Error InvalidPostalCode => Error.Validation("address", "Invalid postal code");
        public static Error InvalidRegion(string region) => Error.Validation("address", $"this region : {region} does not exist ");
        public static Error InvalidCity(string city) => Error.Validation("address", "Invalid city");
        public static Error InvalidProvince(string province) => Error.Validation("address", $"this province : {province} does not exist ");
        public static Error MinimumLength(string propertyName,int minimumLength) => Error.Validation("property.validation",$"{propertyName} must be atleast {minimumLength} characters long");
    }

    public static class CoordinateError
    {
        public static Error InvalidLatitude => Error.Validation("coordinate", "Invalid latitude");
        public static Error InvalidLongitude => Error.Validation("coordinate", "Invalid longitude");
    }

    public static class PhoneNumberError
    {
        public static Error InvalidPhoneNumber => Error.Validation("phoneNumber", "Phone number must start with 0 and followed by 9 digits");
    }

    public static class EmailError
    {
        public static Error InvalidEmailAddress => Error.Validation("email", "Invalid email address");
    }
    public static class SchoolError
    {

    }

}
