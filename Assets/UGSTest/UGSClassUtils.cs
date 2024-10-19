using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGS
{
    public class UGSClassUtils : MonoBehaviour
    {
    }
    public class RaceLobbyParticipant : IDisposable
    {
        public string PlayerID { get; set; }
        public int HorseNumber { get; set; }
        public string PlayerName { get; set; }

        public RaceLobbyParticipant()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
    public class HostScheduleRace : IDisposable
    {
        public string ScheduleStart { get; set; }
        public string ScheduleEnd { get; set; }
        public int TimeGap { get; set; }
        public int PreRaceWaitTime { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
    }
    public class JoinRaceResponse : IDisposable
    {
        public bool CanWaitInLobby { get; set; }
        public string RaceTime { get; set; }
        public string Message { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
        public void Dispose()
        {
            GC.SuppressFinalize(this);
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


    public class VenueRegistrationRequest : IDisposable
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Radius { get; set; }

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
    public class VenueCheckInResponse
    {
        public string Message { get; set; }

        public VenueCheckInResponse()
        {
            Message = string.Empty;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
