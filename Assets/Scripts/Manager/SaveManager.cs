using HorseRace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private int leadChangesToSaverace;
    #endregion

    #region Private Variables
    private HorseRaceResults horseRaceResults;
    private RaceStats raceStat;
    [Tooltip("Key : ControlPointIndex , Value : Race Positions of Horses ")]
    private Dictionary<string, List<int>> saveHorseWaypointCrossDictionary = new Dictionary<string, List<int>>();
    private List<HorseData> horsesData = new List<HorseData>();
    private List<int> horsesInFinishOrder = new List<int>();
    private int preWinnerHorseNumber;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        raceStat = new RaceStats();
        horseRaceResults = new HorseRaceResults();
    }
    private void OnEnable()
    {
        EventManager.Instance.OnRaceFinishEvent += OnRaceFinished;
        EventManager.Instance.OnControlPointChangeEvent += HandleControlPointChange;
    }
  
    private void OnDisable()
    {
        EventManager.Instance.OnRaceFinishEvent -= OnRaceFinished;
        EventManager.Instance.OnControlPointChangeEvent -= HandleControlPointChange;
    }
    #endregion

    #region Public Methods
    public void SetPreDeterminedWinner(int _value)
    {
        preWinnerHorseNumber = _value;
    }
    public void SetWinnersList(List<int> _horsesInRacePositionOrder)
    {
        horsesInFinishOrder = _horsesInRacePositionOrder;
        saveHorseWaypointCrossDictionary.Add("WinnersList", horsesInFinishOrder);
    }
    public void SetHorseData(List<HorseData> _horsesData)
    {
        horsesData = _horsesData;
    }
    #endregion

    #region Subscribed Event Methods
    private void SaveHorseWaypointGroupCrossed(int _horseNumber, int _wayPointGroupIndex)
    {
        List<int> horsePositions = new List<int>();
        if (saveHorseWaypointCrossDictionary.ContainsKey(_wayPointGroupIndex.ToString()))
        {
            horsePositions = saveHorseWaypointCrossDictionary[_wayPointGroupIndex.ToString()];
        }
        horsePositions.Add(_horseNumber);
        saveHorseWaypointCrossDictionary[_wayPointGroupIndex.ToString()] = horsePositions;
    }
    private void HandleControlPointChange(int horseNumber, int controlPointIndex)
    {

    }
    private void OnRaceFinished()
    {
        //Save if the prewinner Wins the race
        //if (IsPreWinnerHorseWinsRace() == false)
        //{
        //    return;
        //}

        //Save if the prewinner horse changes the race position N times
        //if(TotalLeadChangesInRace() < leadChangesToSaverace)
        //{
        //    return;
        //}

        //Remove waypoints positions of horse data
        saveHorseWaypointCrossDictionary.Clear();
        saveHorseWaypointCrossDictionary.Add("WinnersList", horsesInFinishOrder);

        //Create Race Stats
        raceStat.raceIdentifier = Utils.GenerateRandomIdentifier();
        raceStat.predeterminedWinner = horsesInFinishOrder[0];
       // raceStat.predeterminedWinner = preWinnerHorseNumber;
        raceStat.horsesData = horsesData.ToArray();

        ////Save Horse's Waypoint Position Data
        raceStat.waypoints = new Waypoint[saveHorseWaypointCrossDictionary.Keys.Count];
        for (int i = 0; i < raceStat.waypoints.Length; i++)
        {
            raceStat.waypoints[i].number = saveHorseWaypointCrossDictionary.Keys.ElementAt(i);
            raceStat.waypoints[i].positions = new Position[saveHorseWaypointCrossDictionary.Values.ElementAt(i).Count];
            for (int p = 0; p < raceStat.waypoints[i].positions.Length; p++)
            {
                raceStat.waypoints[i].positions[p].position = p + 1;
                raceStat.waypoints[i].positions[p].horseNumber = saveHorseWaypointCrossDictionary.Values.ElementAt(i)[p];
            }
        }

        //Save Stats
        horseRaceResults.SaveRace(raceStat);
    }
    #endregion

    #region Private Methods
    private bool IsPreWinnerHorseWinsRace()
    {
        return horsesInFinishOrder[0] == preWinnerHorseNumber;
    }

    private int TotalLeadChangesInRace()
    {
        int totalLeadChanges = 0;
        int previousLeader = -1;
        int leadChanges = 0;
        for (int i = 0; i < saveHorseWaypointCrossDictionary.Count; i++)
        {
            List<int> horsePositionsAtWaypoints = saveHorseWaypointCrossDictionary[saveHorseWaypointCrossDictionary.Keys.ElementAt(i)];

            for (int j = 0; j < horsePositionsAtWaypoints.Count; j++)
            {
                int currentLeader = horsePositionsAtWaypoints[0];
                if (currentLeader != previousLeader && previousLeader != -1)
                {
                    leadChanges++;
                }
                previousLeader = currentLeader;
            }
            totalLeadChanges += leadChanges;
        }
        //Debug.Log($"\n Total number of lead changes per race:{totalLeadChanges} ");
        return totalLeadChanges;
    }
    #endregion


}
