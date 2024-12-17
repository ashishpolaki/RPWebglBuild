using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UI;

namespace HorseRace
{
    public class RaceManagerLoad : RaceManager
    {
        #region Inspector Variables
        [SerializeField] private LoadManager horseLoadManager;
        [SerializeField] private RaceResultManager raceResultManager;
        [SerializeField] private Transform finishLinePosition;
        [SerializeField] private bool canVerifyRaces;
        #endregion

        #region Private Variables
        private Dictionary<int, int> horsesOvertakeDataDictionary = new Dictionary<int, int>();
        private HorseData[] horseDatas;
        private WaitForSeconds waitForPositionCalculation;
        private int preWinnerHorseNumber;
        private bool isRaceMedalsShown;
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
                base.FixedUpdate();
                ConcludeRaceWithWinner();
            }
        }
        #endregion

        #region Initialize
        public override void Initialize(HorseController[] _horses)
        {
            base.Initialize(_horses);
            OnLoadRaceStats();
            EventManager.Instance.OnCameraSetup();
            UIController.Instance.ScreenEvent(ScreenType.Race, UIScreenEvent.Open);
            waitForPositionCalculation = new WaitForSeconds(GameManager.Instance.HorsesToSpawnList.Count / 12);
        }
        #endregion

        #region Update Horse Race Positions
        IEnumerator IECalculateHorseRacePositions()
        {
            List<KeyValuePair<int, float>> racePositionCalculator = new List<KeyValuePair<int, float>>(horsesByNumber.Count);

            foreach (var horse in horsesByNumber.Values)
            {
                racePositionCalculator.Add(new KeyValuePair<int, float>(horse.HorseNumber, horseSplineManager.GetDistanceCoveredAtSplinePoint(horse.CurrentSplinePointIndex)));
            }
            yield return waitForPositionCalculation;

            // Sort using a custom comparer to avoid LINQ overhead
            racePositionCalculator.Sort((x, y) => y.Value.CompareTo(x.Value));

            // SetRacePositions
            for (int i = 0; i < racePositionCalculator.Count; i++)
            {
                int _horseNumber = racePositionCalculator[i].Key;
                int racePosition = i + 1;

                horsesByNumber[_horseNumber].SetRacePosition(racePosition);
                horsesInRacePositions[racePosition] = _horseNumber;
                horsesTransformInRaceOrder[racePosition] = (_horseNumber, horsesByNumber[_horseNumber].transform);
            }

            // Update UI
            if (horsesInRaceFinishOrder.Count <= 0)
            {
                EventManager.Instance.ShowRacePositions(horsesInRacePositions);
            }
            areRacePositionsCalculating = false;
        }

        protected override void UpdateHorseRacePositions()
        {
            //Update horses state
            foreach (var item in horsesByNumber.Values)
            {
                item.UpdateState();
            }

            //Calculate Horse RacePositions
            if (!areRacePositionsCalculating)
            {
                areRacePositionsCalculating = true;
                StartCoroutine(IECalculateHorseRacePositions());
            }
        }
        #endregion

        #region Horse Transforms
        public override Transform RaceWinnerTransform()
        {
            return horsesByNumber[preWinnerHorseNumber].transform;
        }
        #endregion

        #region Race Winner Medals
        private void ConcludeRaceWithWinner()
        {
            if (horsesInRaceFinishOrder.Count > 0 && !isRaceMedalsShown)
            {
                int winnerHorseNumber = horsesInRaceFinishOrder[0];

                if (horsesByNumber.ContainsKey(winnerHorseNumber))
                {
                     if (horsesByNumber[winnerHorseNumber].CurrentSpeed <= 0.5f)
                    {
                        isRaceMedalsShown = true;
                        int maxRaceWinnersCount = 3;
                        List<HorseControllerLoad> horseControllerLoads = new List<HorseControllerLoad>();
                        for (int i = 0; i < maxRaceWinnersCount && i < horsesByNumber.Count; i++)
                        {
                            horseControllerLoads.Add(horsesByNumber[horsesInRaceFinishOrder[i]] as HorseControllerLoad);
                        }
                        //Store the winner horses Data
                        raceResultManager.InitializeRaceResults(horseControllerLoads);
                        EventManager.Instance.OnWinnersMedals();

                        SoundManager.Instance.StopSound(SoundType.RaceMusic);
                        SoundManager.Instance.StopSound(SoundType.HorseGallop);
                        SoundManager.Instance.StopSound(SoundType.Cheer);
                        // SceneLoadingManager.Instance.LoadNextScene();
                    }
                }
            }
        }
        #endregion

        #region Splines
        public override void ChangeControlPoint(int horseNumber)
        {
            int horseDataArrayIndex = horseNumber - 1;
            ControlPointSave controlPointSave = horseDatas[horseDataArrayIndex].controlPoints[horsesByNumber[horseNumber].CurrentControlPointIndex];
            SplineData splineData = horseSplineManager.GetSplineData(horsesByNumber[horseNumber].CurrentSplineIndex, controlPointSave.splineIndex, horsesByNumber[horseNumber].CurrentControlPointIndex);
            horsesByNumber[horseNumber].SetSpline(splineData);
            horsesByNumber[horseNumber].SetSpeed(controlPointSave.speed, controlPointSave.acceleration);
        }
        #endregion

        #region Race Start/Finish Methods
        protected override void StartRace()
        {
            base.StartRace();
            foreach (var horse in horsesByNumber.Values)
            {
                HorseData horseData = horseDatas[horse.HorseNumber - 1];
                ControlPointSave controlPointSave = horseData.controlPoints[0];
                SplineData splineData = horseSplineManager.InitializeSpline(controlPointSave.splineIndex, horse.HorseNumber);
                horse.InitializeData(splineData, controlPointSave.speed, horseSpeedSO.maxSpeed, controlPointSave.acceleration, horseSplineManager.ChangeThresholdDistance);
            }
        }
        protected override void RaceFinished()
        {
            base.RaceFinished();
            EventManager.Instance.OnRaceWinner(horsesInRaceFinishOrder[0]);
            GameManager.Instance.SetCurrentRaceHorsesOrder(horsesInRaceFinishOrder);
            if (canVerifyRaces)
            {
                VerifyRacePositionList();
            }
        }
        #endregion

        #region Load/Verify Race
        /// <summary>
        /// Verify if race positions are same as saved game race positions
        /// </summary>
        private void VerifyRacePositionList()
        {
            horseLoadManager.VerifyRacePositions(horsesInRaceFinishOrder);
        }

        private void OnLoadRaceStats()
        {
            preWinnerHorseNumber = horseLoadManager.CurrentRaceStat.predeterminedWinner;
            horseDatas = horseLoadManager.CurrentRaceStat.horsesData;

            //int horseNumber = int.Parse(horsesData[i].horseNumber);
            //string[] overtakeDataSplit = horsesData[i].overtakeData.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

            //Load Overtake Data
            //for (int o = 0; o < overtakeDataSplit.Length; o++)
            //{
            //    int velocityIndex = int.Parse(overtakeDataSplit[o]);
            //    if (!horsesOvertakeDataDictionary.ContainsKey(velocityIndex))
            //    {
            //        horsesOvertakeDataDictionary.Add(velocityIndex, horseNumber);
            //    }
            //}
        }
        #endregion
    }
}
