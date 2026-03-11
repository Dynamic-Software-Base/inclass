namespace Application.Common.Utilities;

public static  class GeoDistanceCalculator
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Haversine formula — gives great-circle distance between two points on Earth.
    /// </summary>
    public static double CalculateKm(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = ToRad(lat2 - lat1);
        double dLon  = ToRad(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                   + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                                           * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return Math.Round(EarthRadiusKm * c, 2);
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180;
}
