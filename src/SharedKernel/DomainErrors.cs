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
    public static class SchoolErrors
    {
        public static readonly Error GradeBelongsToDifferentEducationSystem = Error.Validation(
            "School.GradeBelongsToDifferentEducationSystem",
            "The grade definition does not belong to this school's education system.");

        public static readonly Error GradeDefinitionIsInactive = Error.Validation(
            "School.GradeDefinitionIsInactive",
            "Cannot add an inactive grade definition to a school.");

        public static readonly Error OneOrMoreGradeDefinitionsNotFound = Error.Validation(
            "School.OneOrMoreGradeDefinitionsNotFound",
            "One or more grade definitions not found.");

        public static readonly Error GradeAlreadySupported = Error.Conflict(
            "School.GradeAlreadySupported",
            "This grade is already declared as supported by this school.");

        public static readonly Error GradeNotFound = Error.NotFound(
            "School.GradeNotFound",
            "The specified grade is not in this school's supported grades.");
    }

    public static class DateRangeErrors
    {
        public static Error EndsMussBeAfterStart => Error.Validation(
            code: "Domain.DateRange.EndsMussBeAfterStart",
            description: "ends date must be after start date"
        );
    }

    public static class EducationalSystemErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            "EducationalSystem.NotFound",
            "Educational system not found");
    }
}
