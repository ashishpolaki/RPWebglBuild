using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DateTimeUtils 
{
    /// <summary>
    ///  Converts the string to DateTime format
    /// </summary>
    /// <param name="_dateTimeString"></param>
    /// <returns></returns>
    public static DateTime ConvertToUTCTime(string _dateTimeString, string parseFormat)
    {
        DateTime dateTime = DateTime.ParseExact(_dateTimeString, parseFormat, CultureInfo.InvariantCulture);
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, localTimeZone);
        return utcDateTime;
    }

    public static DateTime ConvertUTCTimeToLocalTime(DateTime utcDateTime)
    {
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localTimeZone);
        return localDateTime;
    }

    public static bool IsValidDateTimeFormat(string scheduleString)
    {
        if (DateTime.TryParse(scheduleString, out DateTime dateTime))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool AreDateTimesEqual(DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1 == dateTime2;
    }
}
