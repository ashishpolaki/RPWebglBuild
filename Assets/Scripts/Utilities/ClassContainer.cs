public class ClassContainer
{
    
}


#region Save Race Stats
[System.Serializable]
public class SaveRaceStats
{
    public RaceStats[] raceStats;
}
[System.Serializable]
public class RaceStats
{
    public string raceIdentifier;
    public int predeterminedWinner;
    public Waypoint[] waypoints;
    public HorseData[] horsesData;
}
#endregion
