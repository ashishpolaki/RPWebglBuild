using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HorseRace
{
    public class LoadManager : MonoBehaviour
    {
        private HorseRaceResults horseRaceResults;
     ///   private RaceStats raceStat;
     //   private int currentRaceIndex;
     //   private string currentFileName;

        public RaceStats CurrentRaceStat { get => GameManager.Instance.CurrentRaceData; }

        private void Awake()
        {
            horseRaceResults = new HorseRaceResults();
            // LoadRandomRaceStat();
        }
        private void LoadRandomRaceStat()
        {
          //  (currentRaceIndex, currentFileName, raceStat) = horseRaceResults.LoadRandomRace();
        }

        /// <summary>
        /// Create json file to see race variance positions
        /// </summary>
        /// <param name="_currentHorseNumbers"></param>
        /// <param name="_savedHorseNumbers"></param>
        private void WriteToJson(List<int> _currentHorseNumbers, List<int> _savedHorseNumbers)
        {
            List<VarianceRacePosition> horsePositionsVariances = new List<VarianceRacePosition>();
            bool isHavingVariance = false;

            for (int i = 0; i < _currentHorseNumbers.Count; i++)
            {
                if (_currentHorseNumbers[i] != _savedHorseNumbers[i])
                {
                    VarianceRacePosition racePositionVariance = new VarianceRacePosition();
                    racePositionVariance.racePosition = i + 1;
                    racePositionVariance.savedHorseNumber = _savedHorseNumbers[i];
                    racePositionVariance.currentHorseNumber = _currentHorseNumbers[i];
                    horsePositionsVariances.Add(racePositionVariance);
                    isHavingVariance = true;
                }
            }

            //Save only if having variance
            if (isHavingVariance)
            {
                RaceVarianceResults verifyRaceResults = new RaceVarianceResults();
                verifyRaceResults.raceFileName = GameManager.Instance.CurrentFileName;
                verifyRaceResults.raceIndex = GameManager.Instance.CurrentRaceIndex;
                verifyRaceResults.variances = horsePositionsVariances.ToArray();

                //Change File 
                horseRaceResults.ModifyRaceFile(verifyRaceResults);

                //Save
                string numberString = Regex.Replace(GameManager.Instance.CurrentFileName, "[^0-9]", ""); // Remove all non-numeric characters
                Guid guid = Guid.NewGuid();
                string path = Path.Combine(Application.streamingAssetsPath + "/RaceVarianceFiles", $"{Int32.Parse(numberString)}Race_{guid}.json");
                string json = JsonUtility.ToJson(verifyRaceResults, true);
                File.WriteAllText(path, json);
            }
        }

        public void VerifyRacePositions(List<int> _currentRacePositions)
        {
            Waypoint waypoint = CurrentRaceStat.waypoints[CurrentRaceStat.waypoints.Length - 1];
            for (int i = 0; i < waypoint.positions.Length; i++)
            {
                if (_currentRacePositions[i] != waypoint.positions[i].horseNumber)
                {
                    //Race positions of horses are different from saved race positions
                    WriteToJson(_currentRacePositions, waypoint.positions.Select(x => x.horseNumber).ToList());
                    break;
                }
            }
        }
    }
}
