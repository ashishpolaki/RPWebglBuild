using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HorseCSVManager
{
    private static int raceNumber;
    private static string filePath;

    public void WriteToCSV(int _winnerHorse, List<int> _winnersList, Dictionary<int, List<int>> _waypointCrossDictionary)
    {
        raceNumber++;
        if (raceNumber == 1)
        {
            //Create Horse CSV File
            filePath = Path.Combine(Application.streamingAssetsPath, $"HorseRace{UnityEngine.Random.Range(0, 1000)}.csv");
        }

        var writer = new StreamWriter(filePath, true);
        //Race number and race winner
        writer.WriteLine($"Race,{raceNumber}");
        writer.WriteLine($"Race Winner,Horse {_winnerHorse}");

        //Positions Strings
        writer.Write("Waypoint,");
        string[] positionsArray = new string[12];
        for (int i = 0; i < positionsArray.Length; i++)
        {
            positionsArray[i] = $"Pos {i + 1}";
        }
        string combinedPositionString = string.Join(",", positionsArray);
        writer.WriteLine(combinedPositionString);

        //Waypoints Positions
        for (int i = 0; i < _waypointCrossDictionary.Count; i++)
        {
            //Waypoint Number
            writer.Write($"{_waypointCrossDictionary.Keys.ElementAt(i)},");

            List<int> horsesData = _waypointCrossDictionary[_waypointCrossDictionary.Keys.ElementAt(i)];
            List<string> horseNumbers = new List<string>();

            for (int j = 0; j < horsesData.Count; j++)
            {
                horseNumbers.Add("Horse " + horsesData[j].ToString());
            }
            string combinedPosString = string.Join(",", horseNumbers);
            writer.WriteLine(combinedPosString);
        }

        //Winner List
        writer.Write("WinnersList,");
        var _winnersLists = _winnersList.Select(x => "Horse " + x.ToString()).ToList();
        string combinedWinnersList = string.Join(",", _winnersLists);
        writer.WriteLine(combinedWinnersList);

        writer.Close();
    }
}
