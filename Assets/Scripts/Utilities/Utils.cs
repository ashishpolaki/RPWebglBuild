using System;
using UnityEngine;

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
    public static string ToHex(Color color)
    {
        Color32 color32 = color;
        return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
    }
    public static Color FromHex(string hex)
    {
        if (hex.Length != 8)
        {
            return Color.white;
        }

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, a);
    }
}
