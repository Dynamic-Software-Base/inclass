namespace Application.Common.Errors;

public static class GeocodingErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Geocoding.NotFound",
        "No coordinates found for the provided address.");

    public static readonly Error ServiceUnavailable = Error.Failure(
        "Geocoding.ServiceUnavailable",
        "Geocoding service is currently unavailable.");
}
