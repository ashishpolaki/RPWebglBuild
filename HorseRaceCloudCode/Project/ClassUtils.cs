using System;
using System.Collections.Generic;

namespace HorseRaceCloudCode
{
    internal class ClassUtils
    {
    }

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
        public Dictionary<int, CharacterCustomisationEconomy> playerOutfits { get; set; }

        public StartRaceResponse()
        {
            Message = string.Empty;
            IsRaceStart = false;
            playerOutfits = new Dictionary<int, CharacterCustomisationEconomy>();
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

    #region Lobby
    public class RaceLobbyParticipant
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
    }
    #endregion

    #region Race Result
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

    public class RaceResult
    {
        public List<PlayerRaceResult> playerRaceResults { get; set; }
        public RaceResult()
        {
            playerRaceResults = new List<PlayerRaceResult>();
        }
    }
    #endregion

    #region Race CheckIn
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

    public class RaceCheckInResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public RaceCheckInResponse()
        {
            Message = string.Empty;
            IsSuccess = false;
        }
    }
    #endregion

    #region Venue CheckIn
    public class VenueCheckInResponse
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
    #endregion

    #region Host Schedule/Registration 
    public class VenueRegistrationRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }

        public VenueRegistrationRequest()
        {
            Name = string.Empty;
            DisplayName = string.Empty;
            Latitude = 0;
            Longitude = 0;
            Radius = 0;
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
    #endregion


    #region Character
    public class EconomyCustom
    {
        public EconomyCustom()
        {
        }
    }

    [System.Serializable]
    public class CharacterCustomisationEconomy : EconomyCustom
    {
        public float bodyType;
        public float bodyGenderType;
        public float bodyMuscleType;
        public string skinToneColor = "";
        public List<CustomPartEconomy> customParts = new List<CustomPartEconomy>();
        public UpperOutfitEconomy upperOutfit = new UpperOutfitEconomy();
        public LowerOutfitEconomy lowerOutfit = new LowerOutfitEconomy();

        public CharacterCustomisationEconomy() : base()
        {
            bodyType = 0;
            skinToneColor = "";
            //   customParts = new List<CustomPartEconomy>();
            upperOutfit = new UpperOutfitEconomy();
            lowerOutfit = new LowerOutfitEconomy();
        }
    }
    [System.Serializable]
    public class CustomPartEconomy
    {
        public int type; //(int)BlendPartType Enum
        public int styleNumber; //Part Index
        public string color; //Part Color
                             // public List<BlendShapeEconomy> blendShapes; //Part BlendShapes

        public CustomPartEconomy()
        {
            type = -1;
            styleNumber = -1;
            color = "";
            // blendShapes = new List<BlendShapeEconomy>();
        }
    }


    [System.Serializable]
    public class UpperOutfitEconomy : EconomyCustom
    {
        public int torso;
        public int rightUpperArm;
        public int rightLowerArm;
        public int rightHand;
        public int leftUpperArm;
        public int leftLowerArm;
        public int leftHand;

        public OutfitColorEconomy[] torsoColors;
        public OutfitColorEconomy[] upperArmColors;

        public UpperOutfitEconomy()
        {
            torsoColors = new OutfitColorEconomy[0];
            upperArmColors = new OutfitColorEconomy[0];

            torso = -1;
            rightUpperArm = -1;
            rightLowerArm = 0;
            rightHand = 0;
            leftUpperArm = -1;
            leftLowerArm = 0;
            leftHand = 0;
        }
    }
    [System.Serializable]
    public class LowerOutfitEconomy : EconomyCustom
    {
        public int hips;
        public int rightLeg;
        public int rightFoot;
        public int leftLeg;
        public int leftFoot;

        public OutfitColorEconomy[] hipsColors;
        public OutfitColorEconomy[] legColors;
        public OutfitColorEconomy[] footColors;

        public LowerOutfitEconomy()
        {
            hipsColors = new OutfitColorEconomy[0];
            legColors = new OutfitColorEconomy[0];
            footColors = new OutfitColorEconomy[0];

            hips = -1;
            rightLeg = -1;
            rightFoot = -1;
            leftLeg = -1;
            leftFoot = -1;
        }
    }

    [System.Serializable]
    public class OutfitColorEconomy
    {
        public int u;
        public int v;
        public string color;

        public OutfitColorEconomy()
        {
            u = -1;
            v = -1;
            color = "";
        }
    }
    //[System.Serializable]
    //public class BlendShapeEconomy
    //{
    //    public string name;
    //    public float value;

    //    public BlendShapeEconomy()
    //    {
    //        name = "";
    //        value = 0;
    //    }
    //}
    #endregion
}
