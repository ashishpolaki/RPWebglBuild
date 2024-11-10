using System;
using System.Collections.Generic;

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

    public PlayerRaceData() : base()
    {
        horseNumber = -1;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class RaceData : GameData, IDisposable
{
    //Dictionary of playerID, (playerName, currentDayCheckIns)
    public Dictionary<string, (string,int)> lobbyQualifiedPlayers;
    public List<string> unQualifiedPlayers;

    public DateTime raceTime;
    public int horseNumber;
    public int racePosition;

    public struct CheckInPlayers
    {
        public string playerID;
        public string playerName;
        public int currentDayCheckIns;
    }
    public RaceData() : base()
    {
        lobbyQualifiedPlayers = new Dictionary<string, (string,int)>();
        unQualifiedPlayers = new List<string>();
        raceTime = default;
        horseNumber = 0;
        racePosition = 0;
    }
    public void Dispose()
    {
        if (lobbyQualifiedPlayers != null)
        {
            lobbyQualifiedPlayers.Clear();
            lobbyQualifiedPlayers = null;
        }
        if (unQualifiedPlayers != null)
        {
            unQualifiedPlayers.Clear();
            unQualifiedPlayers = null;
        }
        GC.SuppressFinalize(this);
    }
}
#endregion

