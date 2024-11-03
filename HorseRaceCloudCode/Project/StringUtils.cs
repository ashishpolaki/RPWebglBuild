namespace HorseRaceCloudCode
{
    public static class StringUtils
    {
        #region Keys

        //Venue
        public const string HOSTVENUEKEY = "HostVenue";

        //HOST KEYS
        public const string RACESCHEDULEKEY = "RaceSchedule";
        public const string RACELOBBYKEY = "RaceLobby";
        public const string RACECHECKINKEY = "RaceCheckIn";
        public const string RACERESULTSKEY = "RaceResults";

        #endregion

        public const string HOUR_MINUTE_FORMAT = "HH:mm";
        public const string DAY_FORMAT = "dd";
        public const string YEAR_MONTH_FORMAT = "yyyy-MM";

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
    public class HostConfig
    {
        public static float radiusMin = 1;
        public static float radiusMax = 100;
        public static int maxPlayersInLobby = 12;
        public static int minPlayersInLobby = 2;
        public static int venueCheckInInterval = 15;
    }
}
