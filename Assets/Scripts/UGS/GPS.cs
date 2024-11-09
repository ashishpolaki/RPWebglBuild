using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GPS
{
    #region Properties
    public float Latitude
    {
        get; private set;
    }
    public float Longitude
    {
        get; private set;
    }
    public string Message
    {
        get; private set;
    }
    #endregion

    public delegate void LocationResultHandler(string message, float latitude, float longitude);
    public event LocationResultHandler OnLocationResult;

    public void RequestPermission()
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

    public IEnumerator IEGetLocation()
    {
#if UNITY_EDITOR
        OnLocationResult?.Invoke("Location Fetch Successful",15, 15);
        yield break;
#endif

        const int maxWaitSeconds = 10;
        Message = string.Empty;
        Latitude = 0;
        Longitude = 0;

        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Message = "Location access denied or not enabled";
            OnLocationResult?.Invoke(Message, Latitude, Longitude);
            yield break;
        }

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        for (int waitTime = 0; Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitSeconds; waitTime++)
        {
            yield return new WaitForSeconds(1);
        }

        // If the service didn't initialize in maxWait seconds this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Initializing)
        {
            Message = "Timed out";
            OnLocationResult?.Invoke(Message, Latitude, Longitude);
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Message = "Unable to determine device location";
            OnLocationResult?.Invoke(Message, Latitude, Longitude);
            yield break;
        }

        // Successfully retrieved location
        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        Message = (Latitude != 0 && Longitude != 0) ? "Location Fetch Successful" : "Unable to determine device location";
        OnLocationResult?.Invoke(Message, Latitude, Longitude);

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}
