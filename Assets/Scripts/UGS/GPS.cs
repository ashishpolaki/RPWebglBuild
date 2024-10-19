using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public static class GPS 
{
    public static double CurrentLocationLatitude
    {
        get; private set;
    }
    public static double CurrentLocationLongitude
    {
        get; private set;
    }

    public static void RequestPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
    }

    public static bool IsValidGpsLocation(double latitude, double longitude)
    {
        return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
    }

    public static async Task<bool> TryGetLocationAsync()
    {
        RequestPermission();

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        bool isInitialized = await WaitForLocationServiceToInitialize();
        if (!isInitialized)
        {
            Debug.Log("Timed out");
            return false;
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) || !Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Debug.Log("Location Permission Denied");
            return false;
        }

        // Access granted and location value could be retrieved
        CurrentLocationLatitude = Input.location.lastData.latitude;
        CurrentLocationLongitude = Input.location.lastData.longitude;
        return true;
    }

    private static async Task<bool> WaitForLocationServiceToInitialize()
    {
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            await Task.Delay(1000); // Wait for 1 second
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            return false;
        }

        return true;
    }
}
