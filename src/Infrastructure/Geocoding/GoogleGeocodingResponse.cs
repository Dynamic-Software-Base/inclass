using System.Text.Json.Serialization;

namespace Infrastructure.Geocoding;


internal sealed record GoogleGeocodingResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("results")] GoogleGeocodingResult[] Results
);

internal sealed record GoogleGeocodingResult(
    [property: JsonPropertyName("geometry")] GoogleGeometry Geometry
);

internal sealed record GoogleGeometry(
    [property: JsonPropertyName("location")] GoogleLocation Location
);

internal sealed record GoogleLocation(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);
