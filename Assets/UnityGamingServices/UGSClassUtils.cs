using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGS
{
    public class UGSClassUtils : MonoBehaviour
    {
    }

    #region Lobby
    public class RaceLobbyParticipant : IDisposable
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public string PlayerName { get; set; }

        public RaceLobbyParticipant()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
            HorseNumber = -1;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    #endregion

    #region Host Schedule/Registration 
    public class RaceScheduleRequest : IDisposable
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    public class RaceScheduleResponse : IDisposable
    {
        public bool IsScheduled { get; set; }
        public string Message { get; set; }

        public RaceScheduleResponse()
        {
            IsScheduled = false;
            Message = string.Empty;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
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

    public class VenueRegistrationRequest : IDisposable
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Radius { get; set; }

        public VenueRegistrationRequest()
        {
            Name = string.Empty;
            Latitude = 0;
            Longitude = 0;
            Radius = 0;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    public class VenueRegistrationResponse : IDisposable
    {
        public bool IsRegistered;
        public string Message;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public VenueRegistrationResponse()
        {
            IsRegistered = false;
            Message = string.Empty;
        }
    }

    #endregion

    #region Race Checkins
    public class CurrentRacePlayerCheckIn
    {
        public string PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int CurrentDayCheckIns { get; set; }

        public CurrentRacePlayerCheckIn()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
            CurrentDayCheckIns = 0;
        }
    }

    public class RaceCheckInResponse : IDisposable
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public RaceCheckInResponse()
        {
            Message = string.Empty;
            IsSuccess = false;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    #endregion

    #region Race Result
    public class RaceResult
    {
        public List<PlayerRaceResult> playerRaceResults { get; set; }
        public RaceResult()
        {
            playerRaceResults = new List<PlayerRaceResult>();
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
            HorseNumber = -1;
            RacePosition = -1;
        }
    }
    #endregion

    #region Start Race
    public class StartRaceRequest
    {
        public List<RaceLobbyParticipant> RaceLobbyParticipants { get; set; }
        public List<CurrentRacePlayerCheckIn> UnQualifiedPlayerIDs { get; set; }

        public StartRaceRequest()
        {
            RaceLobbyParticipants = new List<RaceLobbyParticipant>();
            UnQualifiedPlayerIDs = new List<CurrentRacePlayerCheckIn>();
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
    #endregion

    #region Enter race
    public class EnterRaceResponse : IDisposable
    {
        public string Message { get; set; }
        public string UpcomingRaceTime { get; set; }
        public bool IsFoundUpcomingRace { get; set; }
        public bool IsConfirmRaceCheckIn { get; set; }
        public int RaceInterval { get; set; }

        public EnterRaceResponse()
        {
            UpcomingRaceTime = string.Empty;
            Message = string.Empty;
            IsFoundUpcomingRace = false;
            IsConfirmRaceCheckIn = false;
            RaceInterval = 0;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    #endregion

    #region Venue CheckIn

    public class VenueCheckInResponse : IDisposable
    {
        public int CheckInCount { get; set; }
        public string NextCheckInTime { get; set; }
        public string Message { get; set; }
        public bool CanCheckIn { get; set; }
        public bool IsSuccess { get; set; }

        public VenueCheckInResponse()
        {
            Message = string.Empty;
            NextCheckInTime = string.Empty;
            CanCheckIn = false;
            IsSuccess = false;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    #endregion




}
