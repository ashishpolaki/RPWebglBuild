using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HorseRaceResults
{
    private string preFileName = "HorseRaceStats";
    private int racesCount = 10;
    private static string filePath;
    private static List<RaceStats> raceStatsList = new List<RaceStats>();

    #region Public Methods
    /// <summary>
    /// Save Current Race Stats in a Json File
    /// </summary>
    /// <param name="_raceStat"></param>
    public void SaveRace(RaceStats _raceStat)
    {
        //Create a new file if one does not exist, (or)
        //if the current race count in this file is at its maximum.
        if (string.IsNullOrEmpty(filePath) || raceStatsList.Count >= racesCount)
        {
            filePath = GenerateUniqueFileName();
            raceStatsList = new List<RaceStats>();
        }
        raceStatsList.Add(_raceStat);
        SaveRaceStatsData(filePath, raceStatsList.ToArray());
    }

    /// <summary>
    /// Load Random Race from a Json File
    /// </summary>
    /// <returns></returns>
    public (int, string, RaceStats) LoadRandomRace()
    {
        // string searchPattern = "*.json";
        //string[] raceFiles = Directory.GetFiles(Application.streamingAssetsPath, searchPattern);
        //string currentFileName = raceFiles[Utils.GenerateRandomNumber(0, raceFiles.Length)];
        //string currentJsonFile = File.ReadAllText(currentFileName);
        //  return ReturnRaceStats(currentJsonFile, currentFileName);

        // If you need to work with files in the persistent directory, do it here
        TextAsset[] asset = Resources.LoadAll<TextAsset>("RaceFiles");
        TextAsset raceFile = asset[Utils.GenerateRandomNumber(0, asset.Length)];
        string json = raceFile.text;
        return ReturnRaceStats(json, raceFile.name);
    }

    public void ModifyRaceFile(RaceVarianceResults raceVarianceResult)
    {
        // string currentJsonFile = File.ReadAllText(raceVarianceResult.raceFileName);
        string path = $"RaceFiles/{raceVarianceResult.raceFileName}";
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        RaceStats[] races = JsonUtility.FromJson<SaveRaceStats>(textAsset.text).raceStats;
        RaceStats currentRaceStat = races[raceVarianceResult.raceIndex];

        //Change the predetermined winner.
        for (int i = 0; i < raceVarianceResult.variances.Length; i++)
        {
            if (raceVarianceResult.variances[i].racePosition == 1)
            {
                currentRaceStat.predeterminedWinner = raceVarianceResult.variances[i].currentHorseNumber;
            }
        }

        //Change the waypoints
        for (int i = 0; i < currentRaceStat.waypoints.Length; i++)
        {
            if (currentRaceStat.waypoints[i].number == "WinnersList")
            {
                Waypoint waypoint = currentRaceStat.waypoints[i];

                for (int p = 0; p < waypoint.positions.Length; p++)
                {
                    VarianceRacePosition varianceRace = raceVarianceResult.variances.FirstOrDefault(x => x.racePosition == waypoint.positions[p].position);
                    if (varianceRace.racePosition == waypoint.positions[p].position)
                    {
                        waypoint.positions[p].horseNumber = varianceRace.currentHorseNumber;
                    }
                }
            }
        }


        //Now replace the current CurrentRaceData with the modified one. 
        string filePath = Path.Combine(Application.dataPath, "Resources/RaceFiles", raceVarianceResult.raceFileName);
        SaveRaceStatsData(filePath, races);
    }
    #endregion

    #region Private Methods
    private (int, string, RaceStats) ReturnRaceStats(string jsonFile, string currentFileName)
    {
        RaceStats[] races = JsonUtility.FromJson<SaveRaceStats>(jsonFile).raceStats;
        int _raceIndex = Utils.GenerateRandomNumber(0, races.Length);
        RaceStats raceStat = races[_raceIndex];
        return (_raceIndex, currentFileName, raceStat);
    }

    private string GenerateUniqueFileName()
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"{preFileName + Utils.GenerateRandomNumber(0, 10000)}.json");
        return path;
    }

    private void SaveRaceStatsData(string _path, RaceStats[] _raceStats)
    {
        SaveRaceStats saveRaceStats = new SaveRaceStats();
        saveRaceStats.raceStats = _raceStats;
        string json = JsonUtility.ToJson(saveRaceStats, true);
        File.WriteAllText(_path, json);
    }
    #endregion
}

