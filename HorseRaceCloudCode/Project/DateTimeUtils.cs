using System;
using System.Globalization;

namespace HorseRaceCloudCode
{
    public static class DateTimeUtils
    {
        public static DateTime ConvertStringToUTCTime(string dateTimeString)
        {
            DateTime dateTime = DateTime.Parse(dateTimeString);
            return dateTime.ToUniversalTime();
        }
        public static bool IsValidDateTimeFormat(string dateTimeString,string format)
        {
            return DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None,out DateTime time);
        }
        public static bool AreDateTimesEqual(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 == dateTime2;
        }
        public static bool IsValidDateTime(string dateTimeString)
        {
            return DateTime.TryParse(dateTimeString, out DateTime time);
        }
    }
}
