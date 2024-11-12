using System;
using System.Collections.Generic;
using UGS;

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
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float Radius { get; set; }

    public VenueRegistrationData()
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


public class PlayerData : GameData, IDisposable
{
    public string playerID;
    public string playerName;
    public string hostID;
    public string hostVenueName;

    public PlayerData() : base()
    {
        playerID = default;
        playerName = default;
        hostID = default;
        hostVenueName = default;
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

    public HostRaceData() : base()
    {
        qualifiedPlayers = new List<RaceLobbyParticipant>();
        unQualifiedPlayersList = new List<CurrentRacePlayerCheckIn>();
    }   

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
#endregion

