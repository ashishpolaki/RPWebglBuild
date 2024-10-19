using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class RaceAnalyticsProgram
{
    public static void Main(string raceNumber)
    {
        // string filePath = "raceData.json"; // Path to the JSON file
        string filePath = Path.Combine(Application.streamingAssetsPath, $"HorseRaceStats{raceNumber}.json");

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            SaveRaceStats saveRaceStats = JsonUtility.FromJson<SaveRaceStats>(jsonContent);

            if (saveRaceStats?.raceStats == null)
            {
                Debug.Log("No race data found.");
                return;
            }

            int totalRaces = saveRaceStats.raceStats.Length;
            int wins = 0;
            List<int> racePositionChanges = new List<int>();
            Dictionary<int, List<int>> waypointPositions = new Dictionary<int, List<int>>();
            string output = string.Empty;

            foreach (var raceStat in saveRaceStats.raceStats)
            {
                Waypoint lastWaypoint = raceStat.waypoints[raceStat.waypoints.Length - 1];
                Position winningPosition = lastWaypoint.positions[0]; // Assuming the actual winner is the first position in the last waypoint
                int winningHorseNumber = winningPosition.horseNumber;

                if (raceStat.predeterminedWinner == winningHorseNumber)
                {
                    wins++;
                }

                // Track position changes for the predetermined winner in this race
                int previousPosition = -1;
                int positionChanges = 0;

                for (int i = 0; i < raceStat.waypoints.Length; i++)
                {
                    Waypoint waypoint = raceStat.waypoints[i];
                    foreach (var position in waypoint.positions)
                    {
                        if (position.horseNumber == raceStat.predeterminedWinner)
                        {
                            if (!waypointPositions.ContainsKey(i))
                            {
                                waypointPositions[i] = new List<int>();
                            }
                            waypointPositions[i].Add(position.position);

                            if (previousPosition != -1 && position.position != previousPosition)
                            {
                                positionChanges++;
                            }
                            previousPosition = position.position;
                            break; // Move to the next waypoint
                        }
                    }
                }

                racePositionChanges.Add(positionChanges);
            }

            double winningPercentage = (double)wins / totalRaces * 100;
            double averagePositionChange = racePositionChanges.Count > 0 ? (double)racePositionChanges.Sum() / racePositionChanges.Count : 0;

            output += ($"\n The predetermined winner won {wins} out of {totalRaces} races.");
            output += ($"\n Winning Percentage: {winningPercentage}%");
            output += ($"\n Average position changes for the predetermined winner per race: {averagePositionChange:F2}");

            // Calculate and print the average position of the predetermined winner at each waypoint
            foreach (var waypointPosition in waypointPositions)
            {
                double averagePosition = waypointPosition.Value.Count > 0 ? (double)waypointPosition.Value.Sum() / waypointPosition.Value.Count : 0;
                output += ($"\n Average position of the predetermined winner at waypoint {waypointPosition.Key + 1}: {averagePosition:F2}");
            }
            
            int totalLeadChanges = 0;
            Dictionary<int, List<int>> horsePositions = new Dictionary<int, List<int>>();

            foreach (var raceStat in saveRaceStats.raceStats)
            {
                int previousLeader = -1;
                int leadChanges = 0;

                foreach (var waypoint in raceStat.waypoints)
                {
                    Position currentLeader = waypoint.positions[0];
                    if (currentLeader.horseNumber != previousLeader && previousLeader != -1)
                    {
                        leadChanges++;
                    }
                    previousLeader = currentLeader.horseNumber;

                    // Track positions for average calculation
                    foreach (var position in waypoint.positions)
                    {
                        if (!horsePositions.ContainsKey(position.horseNumber))
                        {
                            horsePositions[position.horseNumber] = new List<int>();
                        }
                        horsePositions[position.horseNumber].Add(position.position);
                    }
                }

                totalLeadChanges += leadChanges;
            }

            double averageLeadChanges = (double)totalLeadChanges / saveRaceStats.raceStats.Length;
            output += ($"\n Average number of lead changes per race: {averageLeadChanges:F2}");

            // Print the average position of each horse across all waypoints and races
            foreach (var horsePosition in horsePositions)
            {
                double averagePosition = (double)horsePosition.Value.Sum() / horsePosition.Value.Count;
                output += ($"\n Horse {horsePosition.Key} - Average Position: {averagePosition:F2}");
            }
            Debug.Log(output);
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred: {ex.Message}");
        }
    }
}

// Assuming the classes are defined as in the previous example

