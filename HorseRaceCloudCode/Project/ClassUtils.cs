using System;
using System.Collections.Generic;

namespace HorseRaceCloudCode
{
    internal class ClassUtils
    {
    }

    public class SetVenueNameResponse : IDisposable
    {
        public bool IsVenueNameSet;
        public string Message;

        public SetVenueNameResponse()
        {
            IsVenueNameSet = false;
            Message = string.Empty;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class PlayerVenueCheckIn
    {
        public string Date { get; set; }
        public string LastCheckInTime { get; set; }
        public int Count { get; set; }

        public PlayerVenueCheckIn()
        {
            Date = string.Empty;
            LastCheckInTime = string.Empty;
            Count = 0;
        }
    }
  
    public class StartRaceRequest
    {
        public List<RaceLobbyParticipant> RaceLobbyParticipants { get; set; }
        public List<string> UnQualifiedPlayerIDs { get; set; }

        public StartRaceRequest()
        {
            RaceLobbyParticipants = new List<RaceLobbyParticipant>();
            UnQualifiedPlayerIDs = new List<string>();
        }
    }
    public class StartRaceResponse
    {
        public string Message { get; set; }
        public bool IsRaceStart { get; set; }

        public StartRaceResponse()
        {
            Message = string.Empty;
            IsRaceStart = false;
        }
    }

    public class RaceResult
    {
        public List<PlayerRaceResult> playerRaceResults { get; set; }
        public RaceResult()
        {
            playerRaceResults = new List<PlayerRaceResult>();
        }
    }
    public class JoinRaceResponse
    {
        public bool CanWaitInLobby { get; set; }
        public DateTime RaceTime { get; set; }
        public string Message { get; set; }

        public JoinRaceResponse()
        {
            CanWaitInLobby = false;
            Message = string.Empty;
        }
    }
    public class RaceLobbyParticipant
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public string PlayerName { get; set; }

        public RaceLobbyParticipant()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }
    }
    public class CurrentRacePlayerCheckIn
    {
        public string PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int CurrentDayCheckIns { get; set; }

        public CurrentRacePlayerCheckIn()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }
    }
 
    public class PlayerRaceResult
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public int RacePosition { get; set; }

        public PlayerRaceResult()
        {
            PlayerID = string.Empty;
        }
    }
    public class VenueRegistrationRequest
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }

        public VenueRegistrationRequest()
        {
        }
    }
    public class VenueRegistrationResponse
    {
        public bool IsRegistered { get; set; }
        public string Message { get; set; }

        public VenueRegistrationResponse()
        {
            IsRegistered = false;
            Message = string.Empty;
        }
    }
    public class RaceScheduleRequest
    {
        public string ScheduleStart { get; set; }
        public string ScheduleEnd { get; set; }
        public int RaceInterval { get; set; }
        public int RaceTimings { get; set; }

        public RaceScheduleRequest()
        {
            ScheduleStart = string.Empty;
            ScheduleEnd = string.Empty;
        }
    }
    public class RaceScheduleResponse
    {
        public bool IsScheduled { get; set; }
        public string Message { get; set; }

        public RaceScheduleResponse()
        {
            IsScheduled = false;
            Message = string.Empty;
        }
    }
    public class VenueCheckInResponse
    {
        public string Message { get; set; }

        public VenueCheckInResponse()
        {
            Message = string.Empty;
        }
    }

}
