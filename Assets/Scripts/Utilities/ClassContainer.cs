using System.Collections.Generic;

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

#region Character
public class SaveCharacterData
{
    public int characterGenderIndex;
    public int[] characterPartIndexes;
    public string[] characterPartColorIndexes;

    public SaveCharacterData()
    {
        characterPartIndexes = new int[0];
        characterPartColorIndexes = new string[0];
    }

    public void SetCharacterPartsList(int _characterGenderIndex, int[] characterPartIndexList, string[] characterPartColorList)
    {
        this.characterGenderIndex = _characterGenderIndex;
        this.characterPartIndexes = characterPartIndexList;
        this.characterPartColorIndexes = characterPartColorList;
    }
}
#endregion