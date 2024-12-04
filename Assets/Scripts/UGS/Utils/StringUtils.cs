public class StringUtils
{
    #region UGS Method Names
    public const string HOSTVENUE = "HostVenue";
    public const string PLAYERLOBBY = "Lobby";
    public const string RACESCHEDULE = "RaceSchedule";
    public const string RACECHECKIN = "RaceCheckIn";
    public const string RACELOBBY = "RaceLobby";
    public const string RACERESULT = "RaceResults";
    #endregion

    #region Time Format
    public const string HOUR_MINUTE_AMPM_TIME_FORMAT = "hh:mm tt";
    public const string HOUR_MINUTE_TIME_FORMAT = "HH:mm";
    #endregion

    #region Venue
    public const string VENUE_NAME_EMPTY_ERROR = "Please enter the venue name";
    public const string VENUE_NAME_LENGTH_ERROR = "Venue name should not be more than 15 characters";
    #endregion

    #region RACE SCHEDULE
    public const string INVALID_DATETIME_FORMAT = "Invalid date time format";
    public const string START_AND_END_RACESCHEDULE_EQUAL = "Start and End time cannot be the same";
    public const string ENTER_RACETIMINGS = "Please enter the Race Timings";
    public const string RACE_TIMINGS_GREATERTHANZERO = "The Race Timings should be greater than zero";
    public const string INVALID_RACEINTERVAL_LIMIT = "The Race Interval should be less than the race schedule.";
    public const string ENTER_RACEINTERVAL = "Please enter the Race Interval";
    public const string RACEINTERVAL_GREATERTHANZERO = "Race Interval should be greater than zero";
    public const string RACEINTERVAL_LESSTHAN_RACETIMINGS = "Race Interval should be less than the Race Timings";
    #endregion

    #region GPS 
    public const string GPSLOCATIONFETCHERROR = "Please fetch the current location";
    public const string GPSVALIDLOCATIONERROR = "The GPS location is not valid.";
    public const string GPSRADIUSEMPTY = "Please enter the radius";
    public const string GPSRADIUSMINERROR = "Radius should be greater than one.";
    public const string GPSRADIUSMAXERROR = "Radius should be less than 100.";
    #endregion

    #region lobby
    public static string PLAYERID_EMPTY = "Lobby Player's Unique Id is Empty";
    public static string PLAYERNAME_EMPTY = "Lobby Player's Name is Empty";
    public static string HORSENUMBER_INVALID = "Lobby Player's Horse Number is Invalid";
    public static string MAXIMUM_LOBBYPLAYERS_EXCEEDED = "Maximum Lobby Players Exceeded";
    #endregion

    #region Authentication
    public const string USERNAMEPATTERN = "^[a-zA-Z0-9.\\-@_]{3,20}$";
    public const string PASSWORDPATTERN = "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[\\W]).{8,30}$";
    public const string PLAYERNAMEPATTERN = "^[^\\s]{1,50}$";

    public const string USERNAMEERROR = "Username must be between 3 and 20 characters";
    public const string PASSWORDERROR = "Password must be 8-30 characters long, with at least 1 uppercase, 1 lowercase, 1 number, and 1 symbol.";
    public const string PLAYERNAMEERROR = "Player name should not be more than 50 characters and must not contain spaces.";
    public const string PLAYERFIRSTNAMEERROR  = "Please enter the first name";
    public const string PLAYERLASTNAMEERROR = "Please enter the last name";
    public const string PASSWORDMATCHERROR = "Passwords do not match";
    #endregion

    #region Character
    public const string BLEND_GENDER = "masculineFeminine";
    public const string BLEND_MUSCLE = "defaultBuff";
    public const string BLEND_SHAPE_HEAVY = "defaultHeavy";
    public const string BLEND_SHAPE_SKINNY = "defaultSkinny";
    #endregion


    #region Economy
    public const string INVENTORYITEMID_CHARACTER = "CUSTOMCHARACTER";
    public const string PLAYERINVENTORYITEMID_CHARACTER = "CurrentCharacterData";
    #endregion

    public static bool IsStringEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }
}
