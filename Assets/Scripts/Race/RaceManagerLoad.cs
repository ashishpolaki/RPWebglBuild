using System.Collections.Generic;
using UnityEngine;
using HorseRace.UI;
using System.Linq;
using UnityEngine.AI;
using Unity.Collections;
using Unity.Jobs;

namespace HorseRace
{
    public class RaceManagerLoad : RaceManager
    {
        #region Inspector Variables
        [SerializeField] private LoadManager horseLoadManager;
        [SerializeField] private WinnerHorseJockeyDataSO winnerHorseJockeyDataSO;
        [SerializeField] private Transform finishLinePosition;
        [SerializeField] private bool canVerifyRaces;
        #endregion

        #region Private Variables
        private Dictionary<int, List<Vector3>> horsesVelocityDictionary = new Dictionary<int, List<Vector3>>();
        private Dictionary<int, int> horsesOvertakeDataDictionary = new Dictionary<int, int>();
        private int preWinnerHorseNumber;
        private string preWinnerJockeyName;
        private int horsesVelocityIndex = 0;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetRaceManager(this);
        }
        protected override void FixedUpdate()
        {
            if (isRaceStart)
            {
                LoadHorsePositions();
            }
            base.FixedUpdate();
            ConcludeRaceWithWinner();
        }
        #endregion

        public override void Initialize(HorseController[] _horses)
        {
            base.Initialize(_horses);
            OnLoadRaceStats();
            EventManager.Instance.OnCameraSetup();
        }
        public struct HorseRacePositionRequest
        {
            public int horseNumber;
            public List<Vector3> pathCorners;
        }

        public struct HorseRacePositionResponse
        {
            public int horseNumber;
            public int racePosition;
        }

        public struct HorseRacePositionJob : IJob
        {
            [ReadOnly] public NativeSlice<HorseRacePositionRequest> horseRacePositionRequests;

            public void Execute()
            {
                NativeArray<float> distances = new NativeArray<float>();
                for (int i = 0; i < horseRacePositionRequests.Length; i++)
                {
                    int horseNumber = horseRacePositionRequests[i].horseNumber;

                    //Calculate distance from horses to finish line
                    float distance = 0;
                    for (int j = 0; j < horseRacePositionRequests[i].pathCorners.Count - 1; j++)
                    {
                        distance += Vector3.Distance(horseRacePositionRequests[i].pathCorners[j], horseRacePositionRequests[i].pathCorners[j + 1]);
                    }
                    distances.Append(distance);

                    //Sort the distances of all the horses and 
                }
                //Sort the distances of all the horses and give the race positions accordingly.
                var sortDistances = distances.OrderBy(x => x);

                for (int i = 0; i < sortDistances.ToArray().Length; i++)
                {
                    int horseNumber = horseRacePositionRequests[i].horseNumber;
                    int racePosition = i + 1;
                }
            }
        }


        protected override void UpdateHorseRacePositions()
        {
            //Update Race Positions
            Dictionary<int, float> horseDistances = new Dictionary<int, float>();
            NavMeshPath navMeshPath = new NavMeshPath();
            foreach (var horse in horsesByNumber.Values)
            {
                //Calculate distance from horses to finish line
                navMeshPath = new NavMeshPath();
                float distance = 0;

                NavMesh.CalculatePath(horse.transform.position, finishLinePosition.position, NavMesh.AllAreas, navMeshPath);
                //Add Distances
                for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
                {
                    distance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
                }
                horseDistances.Add(horse.HorseNumber, distance);
            }

            //Sort Horses Distances by ascending order
            var sortDistances = horseDistances.OrderBy(x => x.Value);

            //Set Race Position for Horses
            for (int i = 0; i < horsesByNumber.Count; i++)
            {
                int _horseNumber = sortDistances.ElementAt(i).Key;
                int racePosition = i + 1;
                horsesByNumber[_horseNumber].SetRacePosition(racePosition);
                horsesByNumber[_horseNumber].UpdateState();
                horsesInRacePositions[racePosition] = _horseNumber;
            }

            //Update UI
            if (horsesInFinishOrder.Count <= 0)
            {
                UpdateUI(horsesInRacePositions);
            }
        }
        public override Transform RaceWinnerTransform()
        {
            return horsesByNumber[preWinnerHorseNumber].transform;
        }

        private void ConcludeRaceWithWinner()
        {
            if (horsesInFinishOrder.Count > 0)
            {
                int winnerHorseNumber = horsesInFinishOrder[0];
                if (horsesByNumber.ContainsKey(winnerHorseNumber))
                {
                    if (horsesByNumber[winnerHorseNumber].AgentCurrentSpeed <= 0)
                    {
                        winnerHorseJockeyDataSO.SetMaterials(horsesByNumber[winnerHorseNumber].HorseMaterials, horsesByNumber[winnerHorseNumber].JockeyMaterials);
                        SoundManager.Instance.StopSound(SoundType.RaceMusic);
                        SoundManager.Instance.StopSound(SoundType.HorseGallop);
                        SoundManager.Instance.StopSound(SoundType.Cheer);
                        SceneLoadingManager.Instance.LoadNextScene();
                    }
                }
            }
        }

        #region Race Start/Finish Methods
        protected override void StartRace()
        {
            base.StartRace();
            foreach (var horse in horsesByNumber)
            {
                horse.Value.RaceStart();
            }
        }
        protected override void RaceFinished()
        {
            base.RaceFinished();
            UIManager.Instance.ShowWinnerRaceBoard(preWinnerHorseNumber, preWinnerJockeyName);
            GameManager.Instance.SetCurrentRaceHorsesOrder(horsesInFinishOrder);
            if (canVerifyRaces)
            {
                VerifyRacePositionList();
            }
        }
        protected override void FinishLineCrossed(int _horseNumber)
        {
            base.FinishLineCrossed(_horseNumber);
            if (horsesInFinishOrder.Count > 0)
            {
                Dictionary<int, int> horsesInRacePositions = Enumerable.Range(0, horsesInFinishOrder.Count).ToDictionary(index => index + 1, index => horsesInFinishOrder[index]);
                UIManager.Instance.EnableRacePositions(true);
                UpdateUI(horsesInRacePositions);
            }
        }
        #endregion

        #region UI
        /// <summary>
        /// Update the UI for Horse racing positions.
        /// </summary>
        private void UpdateUI(Dictionary<int, int> racePositions)
        {
            //Update race Positions UI
            UIManager.Instance.UpdateRacePositions(racePositions);
        }
        #endregion

        #region Load/Verify Race

        /// <summary>
        /// Verify if race positions are same as saved game race positions
        /// </summary>
        private void VerifyRacePositionList()
        {
            horseLoadManager.VerifyRacePositions(horsesInFinishOrder);
        }

        /// <summary>
        /// Load horse data and set its velocity
        /// </summary>
        private void LoadHorsePositions()
        {
            foreach (var horse in horsesByNumber)
            {
                int horseNumber = horse.Key;
                if (horsesVelocityIndex < horsesVelocityDictionary[horseNumber].Count)
                {
                    ILoadHorseData loadHorseData = (ILoadHorseData)horsesByNumber[horseNumber];
                    loadHorseData.SetVelocity(horsesVelocityDictionary[horseNumber][horsesVelocityIndex]);
                }
            }

            //Overtake Other horses Data
            //if (horsesOvertakeDataDictionary.ContainsKey(horsesVelocityIndex))
            //{
            //    int horseNumber = horsesOvertakeDataDictionary[horsesVelocityIndex];
            //    if (horseControllers.ContainsKey(horseNumber))
            //    {
            //        CurrentOvertakingHorse = horseControllers[horsesOvertakeDataDictionary[horsesVelocityIndex]];
            //        EventManager.Instance.OnOvertakeCamera(CameraType.Overtake);
            //        horsesOvertakeDataDictionary.Remove(horsesVelocityIndex);
            //    }
            //}
            horsesVelocityIndex++;
        }


        private void OnLoadRaceStats()
        {
            preWinnerHorseNumber = horseLoadManager.CurrentRaceStat.predeterminedWinner;
            HorseData[] horsesData = horseLoadManager.CurrentRaceStat.horsesData;

            char[] sortCharacters = new char[] { '!' };

            //Load the horses Data in a dictionary
            for (int i = 0; i < horsesData.Length; i++)
            {
                int horseNumber = int.Parse(horsesData[i].horseNumber);
                horsesVelocityDictionary[horseNumber] = new List<Vector3>();
                string[] raceDataSplit = horsesData[i].raceData.Split(sortCharacters, System.StringSplitOptions.RemoveEmptyEntries);
                string[] overtakeDataSplit = horsesData[i].overtakeData.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

                //Load Overtake Data
                for (int o = 0; o < overtakeDataSplit.Length; o++)
                {
                    int velocityIndex = int.Parse(overtakeDataSplit[o]);
                    if (!horsesOvertakeDataDictionary.ContainsKey(velocityIndex))
                    {
                        horsesOvertakeDataDictionary.Add(velocityIndex, horseNumber);
                    }
                }

                //Load saved horse velocity List
                for (int j = 0; j < raceDataSplit.Length; j++)
                {
                    string[] splitData = raceDataSplit[j].Split(",", System.StringSplitOptions.RemoveEmptyEntries);
                    Vector3 horseVelocity = new Vector3();
                    horseVelocity.x = float.Parse(splitData[0]);
                    horseVelocity.z = float.Parse(splitData[1]);
                    horsesVelocityDictionary[horseNumber].Add(horseVelocity);
                }
            }
        }
        #endregion
    }
}
