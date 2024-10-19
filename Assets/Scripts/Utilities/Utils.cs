using System;

public class Utils
{
    public static float GenerateRandomNumber(float _min, float _max)
    {
        return UnityEngine.Random.Range(_min, _max);
    }
    public static int GenerateRandomNumber(int _min, int _max)
    {
        return UnityEngine.Random.Range(_min, _max);
    }
    public static string GenerateRandomIdentifier()
    {
        Guid randomGuid = Guid.NewGuid();
        return randomGuid.ToString();
    }
}
