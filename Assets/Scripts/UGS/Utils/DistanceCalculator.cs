using System;

public class DistanceCalculator
{
    public const double EarthRadiusKm = 6371; // Earth radius in kilometers

    /// <summary>
    /// Return Distance in Meters
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="lon1"></param>
    /// <param name="lat2"></param>
    /// <param name="lon2"></param>
    /// <returns></returns>
    public static float CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Pow(Math.Sin(dLon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;
        distance = ConvertKilometersToMeters(distance);
        return (float)distance;
    }

    public static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    public static double ConvertKilometersToMeters(double kilometers)
    {
        return kilometers * 1000; // 1 kilometer is 1000 meters
    }
}
