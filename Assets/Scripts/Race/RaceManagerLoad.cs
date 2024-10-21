using System.Collections.Generic;
using UnityEngine;
using HorseRace.UI;
using System.Linq;
using UnityEngine.AI;
using System.Collections;

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
        private WaitForSeconds waitForPositionCalculation;
        private bool areRacePositionsCalculated = true;
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
            waitForPositionCalculation = new WaitForSeconds(1f / GameManager.Instance.HorsesToSpawnList.Count);
        }

        IEnumerator IECalculateHorseRacePositions()
        {
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
            yield return waitForPositionCalculation;

            // Sort the list by distance using LINQ
            List<KeyValuePair<int, float>> horseDistanceList = horseDistances.OrderBy(h => h.Value).ToList();

            // Set Race Position for Horses
            for (int i = 0; i < horseDistanceList.Count; i++)
            {
                int _horseNumber = horseDistanceList[i].Key;
                int racePosition = i + 1;
                horsesByNumber[_horseNumber].SetRacePosition(racePosition);
                horsesInRacePositions[racePosition] = _horseNumber;
                horsesTransformInRaceOrder[racePosition] = (_horseNumber,horsesByNumber[_horseNumber].transform);
            }

            // Update UI
            if (horsesInFinishOrder.Count <= 0)
            {
                UpdateUI(horsesInRacePositions);
            }
            areRacePositionsCalculated = true;
        }

        protected override void UpdateHorseRacePositions()
        {
            //Update horses state
            for (int i = 0; i < horsesByNumber.Count; i++)
            {
                int _horseNumber = i + 1;
                horsesByNumber[_horseNumber].UpdateState();
            }

            //Calculate Horse RacePositions
            if (areRacePositionsCalculated)
            {
                areRacePositionsCalculated = false;
                StartCoroutine(IECalculateHorseRacePositions());
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
