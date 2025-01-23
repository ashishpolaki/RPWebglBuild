using System;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class GameData
{
    public GameData()
    {
    }
}

#region UGS GameData
public class VenueRegistrationData : GameData, IDisposable
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float Radius { get; set; }

    public VenueRegistrationData()
    {
        Name = string.Empty;
        DisplayName = string.Empty;
        Latitude = 0;
        Longitude = 0;
        Radius = 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}


public class PlayerData : GameData, IDisposable
{
    public string playerID;
    public string playerName;
    public string hostID;
    public string hostVenueName;
    public bool isHost;
    public Character character;

    public PlayerData() : base()
    {
        playerID = default;
        playerName = default;
        hostID = default;
        isHost = false;
        hostVenueName = default;
        character = null;
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class PlayerRaceData : GameData, IDisposable
{
    public int horseNumber;
    public DateTime upcomingRaceTime;
    public int raceInterval;
    public int racePosition;

    public PlayerRaceData() : base()
    {
        horseNumber = -1;
        racePosition = -1;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class HostRaceData : GameData, IDisposable
{
    public List<RaceLobbyParticipant> qualifiedPlayers;
    public List<CurrentRacePlayerCheckIn> unQualifiedPlayersList;
    // [Tooltip("Key : HorseNumber, Value : CharacterCustomisationData")]
    public Dictionary<int, CharacterCustomisationEconomy> characterCustomisationDatas;
    // [Tooltip("Key : HorseNumber, Value : HorseCustomisationData")]
    public Dictionary<int, HorseCustomisationEconomy> horseCustomisationDatas;
    public Dictionary<int, RenderTexture> currentRaceAvatars;

    public HostRaceData() : base()
    {
        currentRaceAvatars = new Dictionary<int, RenderTexture>();
        qualifiedPlayers = new List<RaceLobbyParticipant>();
        unQualifiedPlayersList = new List<CurrentRacePlayerCheckIn>();
        characterCustomisationDatas = new Dictionary<int, CharacterCustomisationEconomy>();
    }

    public void Dispose()
    {
        currentRaceAvatars = null;
        qualifiedPlayers = null;
        unQualifiedPlayersList = null;
        characterCustomisationDatas = null;
        GC.SuppressFinalize(this);
    }
}
#endregion

