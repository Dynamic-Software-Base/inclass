namespace Infrastructure.Geocoding;

public sealed class GoogleGeocodingOptions
{
    public const string SectionName = "GoogleGeocoding";
    public string Apikey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://maps.googleapis.com/maps/api/geocode/json";
}
