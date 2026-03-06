namespace SharedKernel.ValueObjects;

public record Coordinates
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private Coordinates() { }

    private  Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static ErrorOr<Coordinates> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            return DomainErrors.CoordinateError.InvalidLatitude;
        }

        if (longitude is < -180 or > 180)
        {
            return DomainErrors.CoordinateError.InvalidLongitude;
        }

        return new Coordinates(latitude, longitude);
    }
}
