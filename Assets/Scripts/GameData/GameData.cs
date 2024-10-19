using System;
using System.Collections.Generic;

public class GameData
{
    public GameData()
    {
    }
}

#region UGS GameData
public class PlayerData : GameData, IDisposable
{
    public string playerID;
    public string playerName;
    public string hostID;

    public PlayerData() : base()
    {
        playerID = default;
        playerName = default;
        hostID = default;
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
public class LocationData : GameData, IDisposable
{
    public double latitude;
    public double longitude;

    public LocationData() : base()
    {
        latitude = default;
        longitude = default;
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

